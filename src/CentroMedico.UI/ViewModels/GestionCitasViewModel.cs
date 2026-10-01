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
    public class GestionCitasViewModel : ViewModelBase
    {
        private readonly ICitaService _servicio;

        public ObservableCollection<Cita> citas { get; }
            = new ObservableCollection<Cita>();

        public ObservableCollection<Paciente> pacientes { get; }
            = new ObservableCollection<Paciente>();

        public ObservableCollection<Medico> medicos { get; }
            = new ObservableCollection<Medico>();

        public RelayCommand ComandoCargar { get; }
        public RelayCommand ComandoNuevo { get; }
        public RelayCommand ComandoRegistrar { get; }
        public RelayCommand ComandoReprogramar { get; }
        public RelayCommand ComandoCancelar { get; }

        public GestionCitasViewModel(ICitaService servicio)
        {
            _servicio = servicio;

            ComandoCargar =
                new RelayCommand(Cargar, () => disponible);

            ComandoNuevo =
                new RelayCommand(Nuevo, () => disponible);

            ComandoRegistrar =
                new RelayCommand(
                    Registrar,
                    () => disponible &&
                          citaSeleccionada == null);

            ComandoReprogramar =
                new RelayCommand(
                    Reprogramar,
                    () => disponible &&
                          citaSeleccionada != null &&
                          citaSeleccionada.Estado == "PENDIENTE");

            ComandoCancelar =
                new RelayCommand(
                    Cancelar,
                    () => disponible &&
                          citaSeleccionada != null &&
                          citaSeleccionada.Estado == "PENDIENTE");

            Nuevo();
        }

        private bool _ocupado;

        public bool ocupado
        {
            get => _ocupado;
            private set
            {
                _ocupado = value;
                OnPropertyChanged(nameof(ocupado));
                OnPropertyChanged(nameof(disponible));
                NotificarComandos();
            }
        }

        public bool disponible => !ocupado;

        private Cita? _citaSeleccionada;

        public Cita? citaSeleccionada
        {
            get => _citaSeleccionada;
            set
            {
                if (_citaSeleccionada == value)
                    return;

                _citaSeleccionada = value;
                OnPropertyChanged(nameof(citaSeleccionada));

                if (_citaSeleccionada != null)
                    CargarCitaSeleccionada();

                NotificarComandos();
            }
        }

        private Paciente? _pacienteSeleccionado;

        public Paciente? pacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set
            {
                _pacienteSeleccionado = value;
                OnPropertyChanged(nameof(pacienteSeleccionado));
            }
        }

        private Medico? _medicoSeleccionado;

        public Medico? medicoSeleccionado
        {
            get => _medicoSeleccionado;
            set
            {
                _medicoSeleccionado = value;
                OnPropertyChanged(nameof(medicoSeleccionado));
            }
        }

        private string _codigo = string.Empty;

        public string codigo
        {
            get => _codigo;
            set
            {
                _codigo = value;
                OnPropertyChanged(nameof(codigo));
            }
        }

        private DateTime? _fecha;

        public DateTime? fecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged(nameof(fecha));
            }
        }

        private string _hora = "09:00";

        public string hora
        {
            get => _hora;
            set
            {
                _hora = value;
                OnPropertyChanged(nameof(hora));
            }
        }

        private string _tarifa = string.Empty;

        public string tarifa
        {
            get => _tarifa;
            set
            {
                _tarifa = value;
                OnPropertyChanged(nameof(tarifa));
            }
        }

        private string _mensaje = string.Empty;

        public string mensaje
        {
            get => _mensaje;
            set
            {
                _mensaje = value;
                OnPropertyChanged(nameof(mensaje));
            }
        }

        private async void Cargar()
        {
            await CargarDatosAsync();
        }

        public async Task CargarDatosAsync()
        {
            if (ocupado)
                return;

            ocupado = true;

            try
            {
                await CargarListasAsync();
                Nuevo();
                mensaje = "Datos de la agenda cargados.";
            }
            catch (Exception ex)
            {
                mensaje = "No se pudieron cargar las citas: "
                          + ex.Message;
            }
            finally
            {
                ocupado = false;
            }
        }

        private async Task CargarListasAsync()
        {
            List<Paciente> listaPacientes =
                await _servicio.ListarPacientesAsync();

            List<Medico> listaMedicos =
                await _servicio.ListarMedicosAsync();

            List<Cita> listaCitas =
                await _servicio.ListarCitasAsync();

            pacientes.Clear();
            medicos.Clear();
            citas.Clear();

            foreach (Paciente paciente in listaPacientes)
                pacientes.Add(paciente);

            foreach (Medico medico in listaMedicos)
                medicos.Add(medico);

            foreach (Cita cita in listaCitas)
                citas.Add(cita);
        }

        private void Nuevo()
        {
            citaSeleccionada = null;
            pacienteSeleccionado = null;
            medicoSeleccionado = null;

            codigo = "CITA-"
                + DateTime.Now.ToString("yyyyMMddHHmmss");

            fecha = DateTime.Today.AddDays(1);
            hora = "09:00";
            tarifa = string.Empty;
            mensaje = "Complete los datos de la nueva cita.";

            NotificarComandos();
        }

        private void CargarCitaSeleccionada()
        {
            if (citaSeleccionada == null)
                return;

            codigo = citaSeleccionada.Codigo;

            pacienteSeleccionado = pacientes.FirstOrDefault(
                x => x.Id == citaSeleccionada.Paciente.Id);

            medicoSeleccionado = medicos.FirstOrDefault(
                x => x.Id == citaSeleccionada.Medico.Id);

            fecha = citaSeleccionada.Fecha.Date;

            hora = citaSeleccionada.Fecha.ToString("HH:mm");

            tarifa = citaSeleccionada.Tarifa.ToString(
                "0.00",
                CultureInfo.CurrentCulture);
        }

        private async void Registrar()
        {
            if (ocupado)
                return;

            ocupado = true;

            try
            {
                Cita nuevaCita = ConstruirCita();

                int citaId =
                    await _servicio.RegistrarCitaAsync(nuevaCita);

                await CargarListasAsync();
                Nuevo();

                mensaje = "Cita registrada correctamente. ID: "
                          + citaId + ".";
            }
            catch (Exception ex)
            {
                mensaje = "No se pudo registrar la cita: "
                          + ex.Message;
            }
            finally
            {
                ocupado = false;
            }
        }

        private async void Reprogramar()
        {
            if (ocupado || citaSeleccionada == null)
                return;

            ocupado = true;

            try
            {
                Cita citaActualizada = ConstruirCita();
                citaActualizada.Id = citaSeleccionada.Id;

                await _servicio.ReprogramarCitaAsync(
                    citaActualizada);

                await CargarListasAsync();
                Nuevo();

                mensaje = "Cita reprogramada correctamente.";
            }
            catch (Exception ex)
            {
                mensaje = "No se pudo reprogramar la cita: "
                          + ex.Message;
            }
            finally
            {
                ocupado = false;
            }
        }

        private async void Cancelar()
        {
            if (ocupado || citaSeleccionada == null)
                return;

            ocupado = true;

            try
            {
                int citaId = citaSeleccionada.Id;

                await _servicio.CancelarCitaAsync(citaId);

                await CargarListasAsync();
                Nuevo();

                mensaje = "Cita cancelada correctamente.";
            }
            catch (Exception ex)
            {
                mensaje = "No se pudo cancelar la cita: "
                          + ex.Message;
            }
            finally
            {
                ocupado = false;
            }
        }

        private Cita ConstruirCita()
        {
            if (pacienteSeleccionado == null)
                throw new ArgumentException(
                    "Seleccione un paciente.");

            if (medicoSeleccionado == null)
                throw new ArgumentException(
                    "Seleccione un médico.");

            if (fecha == null)
                throw new ArgumentException(
                    "Seleccione una fecha.");

            if (!TimeSpan.TryParse(
                    hora,
                    CultureInfo.CurrentCulture,
                    out TimeSpan horaCita)
                || horaCita < TimeSpan.Zero
                || horaCita.TotalHours >= 24)
            {
                throw new ArgumentException(
                    "Ingrese una hora válida. Ejemplo: 09:30.");
            }

            if (!decimal.TryParse(
                    tarifa,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal valorTarifa)
                || valorTarifa <= 0)
            {
                throw new ArgumentException(
                    "Ingrese una tarifa válida.");
            }

            DateTime fechaCompleta =
                fecha.Value.Date.Add(horaCita);

            return new Cita
            {
                Codigo = codigo,
                Paciente = pacienteSeleccionado,
                Medico = medicoSeleccionado,
                Fecha = fechaCompleta,
                Tarifa = valorTarifa,
                Estado = "PENDIENTE"
            };
        }

        private void NotificarComandos()
        {
            ComandoCargar.NotificarCanExecuteChanged();
            ComandoNuevo.NotificarCanExecuteChanged();
            ComandoRegistrar.NotificarCanExecuteChanged();
            ComandoReprogramar.NotificarCanExecuteChanged();
            ComandoCancelar.NotificarCanExecuteChanged();
        }
    }
}