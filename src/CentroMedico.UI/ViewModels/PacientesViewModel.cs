using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.UI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CentroMedico.UI.ViewModels
{
    public class PacientesViewModel
        : ViewModelBase
    {
        private readonly IPacienteService
            _service;

        private Paciente? _pacienteSeleccionado;
        private string _documento = string.Empty;
        private string _nombre = string.Empty;
        private DateTime? _fechaNacimiento;
        private string? _telefono;
        private string _mensaje = string.Empty;

        public ObservableCollection<Paciente>
            Pacientes
        { get; } = new();

        public Paciente? PacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set
            {
                if (_pacienteSeleccionado == value)
                    return;

                _pacienteSeleccionado = value;

                OnPropertyChanged(
                    nameof(PacienteSeleccionado));

                CargarPacienteSeleccionado();
            }
        }

        public string Documento
        {
            get => _documento;
            set
            {
                if (_documento == value)
                    return;

                _documento = value;

                OnPropertyChanged(
                    nameof(Documento));
            }
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre == value)
                    return;

                _nombre = value;

                OnPropertyChanged(
                    nameof(Nombre));
            }
        }

        public DateTime? FechaNacimiento
        {
            get => _fechaNacimiento;
            set
            {
                if (_fechaNacimiento == value)
                    return;

                _fechaNacimiento = value;

                OnPropertyChanged(
                    nameof(FechaNacimiento));
            }
        }

        public string? Telefono
        {
            get => _telefono;
            set
            {
                if (_telefono == value)
                    return;

                _telefono = value;

                OnPropertyChanged(
                    nameof(Telefono));
            }
        }

        public string Mensaje
        {
            get => _mensaje;
            set
            {
                if (_mensaje == value)
                    return;

                _mensaje = value;

                OnPropertyChanged(
                    nameof(Mensaje));
            }
        }

        public RelayCommand ComandoCargar
        {
            get;
        }

        public RelayCommand ComandoNuevo
        {
            get;
        }

        public RelayCommand ComandoRegistrar
        {
            get;
        }

        public RelayCommand ComandoActualizar
        {
            get;
        }

        public PacientesViewModel(
            IPacienteService service)
        {
            _service = service;

            ComandoCargar = new RelayCommand(
                async () => await CargarAsync());

            ComandoNuevo = new RelayCommand(
                Nuevo);

            ComandoRegistrar = new RelayCommand(
                async () => await RegistrarAsync());

            ComandoActualizar = new RelayCommand(
                async () => await ActualizarAsync());

            LimpiarFormulario();
        }

        public async Task CargarAsync()
        {
            try
            {
                var registros =
                    await _service.ListarAsync();

                Pacientes.Clear();

                foreach (Paciente paciente
                    in registros)
                {
                    Pacientes.Add(paciente);
                }

                Mensaje =
                    "Pacientes cargados correctamente.";
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No se pudieron cargar los pacientes: "
                    + ex.Message;
            }
        }

        private void Nuevo()
        {
            LimpiarFormulario();

            Mensaje =
                "Complete los datos del nuevo paciente.";
        }

        private async Task RegistrarAsync()
        {
            try
            {
                Paciente paciente =
                    CrearPacienteFormulario();

                int id =
                    await _service.RegistrarAsync(
                        paciente);

                await CargarAsync();

                LimpiarFormulario();

                Mensaje =
                    "Paciente registrado correctamente. ID: "
                    + id + ".";
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }

        private async Task ActualizarAsync()
        {
            if (PacienteSeleccionado == null)
            {
                Mensaje =
                    "Seleccione un paciente de la lista.";

                return;
            }

            try
            {
                Paciente paciente =
                    CrearPacienteFormulario();

                paciente.Id =
                    PacienteSeleccionado.Id;

                await _service.ActualizarAsync(
                    paciente);

                await CargarAsync();

                LimpiarFormulario();

                Mensaje =
                    "Paciente actualizado correctamente.";
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }

        private Paciente CrearPacienteFormulario()
        {
            return new Paciente
            {
                Documento = Documento,
                Nombre = Nombre,

                FechaNacimiento =
                    FechaNacimiento ?? default,

                Telefono = Telefono
            };
        }

        private void CargarPacienteSeleccionado()
        {
            if (PacienteSeleccionado == null)
                return;

            Documento =
                PacienteSeleccionado.Documento;

            Nombre =
                PacienteSeleccionado.Nombre;

            FechaNacimiento =
                PacienteSeleccionado.FechaNacimiento;

            Telefono =
                PacienteSeleccionado.Telefono;

            Mensaje =
                "Paciente seleccionado para actualizar.";
        }

        private void LimpiarFormulario()
        {
            _pacienteSeleccionado = null;

            OnPropertyChanged(
                nameof(PacienteSeleccionado));

            Documento = string.Empty;
            Nombre = string.Empty;
            FechaNacimiento = DateTime.Today;
            Telefono = string.Empty;
        }
    }
}