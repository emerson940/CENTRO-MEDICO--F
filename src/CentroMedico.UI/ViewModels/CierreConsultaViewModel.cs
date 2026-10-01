using CentroMedico.Application.DTOs;
using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.UI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CentroMedico.UI.ViewModels
{
    public class CierreConsultaViewModel : ViewModelBase
    {
        private readonly ICierreConsultaService _servicio;
        public ObservableCollection<Cita> citas { get; } = new ObservableCollection<Cita>();
        public ObservableCollection<Insumo> insumos { get; } = new ObservableCollection<Insumo>();
        public ObservableCollection<DetalleReceta> medicamentos { get; } = new ObservableCollection<DetalleReceta>();
        public ObservableCollection<ConsumoInsumo> insumosUtilizados { get; } = new ObservableCollection<ConsumoInsumo>();

        public RelayCommand ComandoCargar { get; }
        public RelayCommand ComandoAgregarMedicamento { get; }
        public RelayCommand ComandoQuitarMedicamento { get; }
        public RelayCommand ComandoAgregarInsumo { get; }
        public RelayCommand ComandoQuitarInsumo { get; }
        public RelayCommand ComandoCerrarConsulta { get; }
        public RelayCommand ComandoNuevo { get; }

        public CierreConsultaViewModel(ICierreConsultaService servicio)
        {
            _servicio = servicio;
            ComandoCargar = new RelayCommand(Cargar, () => disponible);
            ComandoAgregarMedicamento = new RelayCommand(AgregarMedicamento, () => disponible);
            ComandoQuitarMedicamento = new RelayCommand(QuitarMedicamento, () => disponible && medicamentoSeleccionado != null);
            ComandoAgregarInsumo = new RelayCommand(AgregarInsumo, () => disponible);
            ComandoQuitarInsumo = new RelayCommand(QuitarInsumo, () => disponible && consumoSeleccionado != null);
            ComandoCerrarConsulta = new RelayCommand(CerrarConsulta, () => disponible && citaSeleccionada != null);
            ComandoNuevo = new RelayCommand(Nuevo, () => disponible);
        }

        private bool _ocupado;
        public bool ocupado
        {
            get { return _ocupado; }
            private set
            {
                _ocupado = value;
                OnPropertyChanged(nameof(ocupado));
                OnPropertyChanged(nameof(disponible));
                NotificarComandos();
            }
        }
        public bool disponible { get { return !ocupado; } }

        private Cita? _citaSeleccionada;
        public Cita? citaSeleccionada
        {
            get { return _citaSeleccionada; }
            set
            {
                if (_citaSeleccionada == value) return;
                _citaSeleccionada = value;
                OnPropertyChanged(nameof(citaSeleccionada));
                LimpiarCampos();
                ComandoCerrarConsulta.NotificarCanExecuteChanged();
            }
        }
        private Insumo? _insumoSeleccionado;
        public Insumo? insumoSeleccionado
        {
            get { return _insumoSeleccionado; }
            set { _insumoSeleccionado = value; OnPropertyChanged(nameof(insumoSeleccionado)); }
        }
        private DetalleReceta? _medicamentoSeleccionado;
        public DetalleReceta? medicamentoSeleccionado
        {
            get { return _medicamentoSeleccionado; }
            set
            {
                _medicamentoSeleccionado = value;
                OnPropertyChanged(nameof(medicamentoSeleccionado));
                ComandoQuitarMedicamento.NotificarCanExecuteChanged();
            }
        }
        private ConsumoInsumo? _consumoSeleccionado;
        public ConsumoInsumo? consumoSeleccionado
        {
            get { return _consumoSeleccionado; }
            set
            {
                _consumoSeleccionado = value;
                OnPropertyChanged(nameof(consumoSeleccionado));
                ComandoQuitarInsumo.NotificarCanExecuteChanged();
            }
        }

        private string _diagnostico = string.Empty;
        public string diagnostico
        {
            get { return _diagnostico; }
            set { _diagnostico = value; OnPropertyChanged(nameof(diagnostico)); }
        }

        private string _observaciones = string.Empty;
        public string observaciones
        {
            get { return _observaciones; }
            set { _observaciones = value; OnPropertyChanged(nameof(observaciones)); }
        }

        private string _indicaciones = string.Empty;
        public string indicaciones
        {
            get { return _indicaciones; }
            set { _indicaciones = value; OnPropertyChanged(nameof(indicaciones)); }
        }

        private string _medicamento = string.Empty;
        public string medicamento
        {
            get { return _medicamento; }
            set { _medicamento = value; OnPropertyChanged(nameof(medicamento)); }
        }

        private string _dosis = string.Empty;
        public string dosis
        {
            get { return _dosis; }
            set { _dosis = value; OnPropertyChanged(nameof(dosis)); }
        }

        private string _frecuencia = string.Empty;
        public string frecuencia
        {
            get { return _frecuencia; }
            set { _frecuencia = value; OnPropertyChanged(nameof(frecuencia)); }
        }

        private string _duracion = string.Empty;
        public string duracion
        {
            get { return _duracion; }
            set { _duracion = value; OnPropertyChanged(nameof(duracion)); }
        }

        private string _cantidad = string.Empty;
        public string cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; OnPropertyChanged(nameof(cantidad)); }
        }

        private string _mensaje = string.Empty;
        public string mensaje
        {
            get { return _mensaje; }
            set { _mensaje = value; OnPropertyChanged(nameof(mensaje)); }
        }

        private async void Cargar()
        {
            await CargarDatosAsync();
        }

        public async Task CargarDatosAsync()
        {
            if (ocupado) return;
            ocupado = true;
            try
            {
                await CargarListasAsync();
                mensaje = "Seleccione una cita pendiente.";
            }
            catch (Exception ex)
            {
                mensaje = "No se pudieron cargar los datos: " + ex.Message;
            }
            finally { ocupado = false; }
        }

        private async Task CargarListasAsync()
        {
            List<Cita> listaCitas = await _servicio.ListarCitasPendientesAsync();
            List<Insumo> listaInsumos = await _servicio.ListarInsumosAsync();
            citaSeleccionada = null;
            insumoSeleccionado = null;
            citas.Clear();
            insumos.Clear();
            foreach (Cita cita in listaCitas) citas.Add(cita);
            foreach (Insumo insumo in listaInsumos) insumos.Add(insumo);
        }

        private void AgregarMedicamento()
        {
            string[] campos = { medicamento, dosis, frecuencia, duracion };
            if (campos.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 100))
            {
                mensaje = "Complete medicamento, dosis, frecuencia y duración (máximo 100 caracteres por campo).";
                return;
            }
            medicamentos.Add(new DetalleReceta
            {
                Medicamento = medicamento.Trim(), Dosis = dosis.Trim(),
                Frecuencia = frecuencia.Trim(), Duracion = duracion.Trim()
            });
            medicamento = dosis = frecuencia = duracion = string.Empty;
            mensaje = "Medicamento agregado a la receta.";
        }

        private void QuitarMedicamento()
        {
            if (medicamentoSeleccionado != null) medicamentos.Remove(medicamentoSeleccionado);
            medicamentoSeleccionado = null;
        }

        private void AgregarInsumo()
        {
            if (insumoSeleccionado == null)
            {
                mensaje = "Seleccione un insumo.";
                return;
            }
            if (!decimal.TryParse(cantidad.Trim(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.CurrentCulture, out decimal unidades) || unidades <= 0 ||
                unidades > 9999999999.99m || decimal.Round(unidades, 2) != unidades)
            {
                mensaje = "Ingrese una cantidad positiva con máximo dos decimales.";
                return;
            }
            if (insumosUtilizados.Any(x => x.InsumoId == insumoSeleccionado.Id))
            {
                mensaje = "Ese insumo ya está agregado. Quítelo y vuelva a agregarlo con la cantidad total.";
                return;
            }
            insumosUtilizados.Add(new ConsumoInsumo
            {
                InsumoId = insumoSeleccionado.Id, Nombre = insumoSeleccionado.Nombre, Cantidad = unidades
            });
            cantidad = string.Empty;
            mensaje = "Insumo agregado. Las existencias se verificarán durante el cierre.";
        }

        private void QuitarInsumo()
        {
            if (consumoSeleccionado != null) insumosUtilizados.Remove(consumoSeleccionado);
            consumoSeleccionado = null;
        }

        // async void se usa solo como entrada del comando Action de clase.
        // Todas las excepciones de la operación asíncrona se capturan aquí.
        private async void CerrarConsulta()
        {
            if (ocupado || citaSeleccionada == null) return;
            ocupado = true;
            try
            {
                CierreConsultaDTO datos = new CierreConsultaDTO
                {
                    CitaId = citaSeleccionada.Id,
                    RowVersion = citaSeleccionada.RowVersion.ToArray(),
                    Diagnostico = diagnostico,
                    Observaciones = observaciones,
                    Indicaciones = indicaciones,
                    Medicamentos = medicamentos.ToList(),
                    Insumos = insumosUtilizados.ToList()
                };

                OrdenCobro orden = await _servicio.CerrarConsultaAsync(datos);

                citaSeleccionada = null;
                LimpiarCampos();
                mensaje = $"Consulta cerrada. Orden de cobro N.º {orden.Id}. Total: S/ {orden.Total:N2}. Pago pendiente.";
                // La recarga ocurre después del Commit. Su fallo no implica que el cierre falló.
                try { await CargarListasAsync(); }
                catch (Exception ex) { mensaje += " El cierre se guardó, pero no se pudo recargar la agenda: " + ex.Message; }
            }
            catch (Exception ex)
            {
                mensaje = "No se completó el cierre: " + ex.Message;
            }
            finally { ocupado = false; }
        }

        private void Nuevo()
        {
            citaSeleccionada = null;
            LimpiarCampos();
            mensaje = "Seleccione una cita pendiente.";
        }

        private void LimpiarCampos()
        {
            diagnostico = observaciones = indicaciones = string.Empty;
            medicamento = dosis = frecuencia = duracion = cantidad = string.Empty;
            medicamentos.Clear();
            insumosUtilizados.Clear();
            medicamentoSeleccionado = null;
            consumoSeleccionado = null;
        }

        private void NotificarComandos()
        {
            ComandoCargar.NotificarCanExecuteChanged();
            ComandoAgregarMedicamento.NotificarCanExecuteChanged();
            ComandoQuitarMedicamento.NotificarCanExecuteChanged();
            ComandoAgregarInsumo.NotificarCanExecuteChanged();
            ComandoQuitarInsumo.NotificarCanExecuteChanged();
            ComandoCerrarConsulta.NotificarCanExecuteChanged();
            ComandoNuevo.NotificarCanExecuteChanged();
        }
    }
}
