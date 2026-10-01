using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.UI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CentroMedico.UI.ViewModels
{
    public class MedicosViewModel : ViewModelBase
    {
        private readonly IMedicoService _service;

        public ObservableCollection<Medico> medicos
        {
            get;
        } = new();

        private Medico? _medicoSeleccionado;

        public Medico? medicoSeleccionado
        {
            get => _medicoSeleccionado;
            set
            {
                _medicoSeleccionado = value;

                OnPropertyChanged(
                    nameof(medicoSeleccionado));

                if (value != null)
                {
                    codigo = value.Codigo;
                    nombre = value.Nombre;
                    especialidad = value.Especialidad;
                }

                NotificarComandos();
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

        private string _nombre = string.Empty;

        public string nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(nombre));
            }
        }

        private string _especialidad = string.Empty;

        public string especialidad
        {
            get => _especialidad;
            set
            {
                _especialidad = value;

                OnPropertyChanged(
                    nameof(especialidad));
            }
        }

        private string _mensaje =
            "Cargue la lista o registre un nuevo médico.";

        public string mensaje
        {
            get => _mensaje;
            set
            {
                _mensaje = value;
                OnPropertyChanged(nameof(mensaje));
            }
        }

        private bool _disponible = true;

        public bool disponible
        {
            get => _disponible;
            private set
            {
                _disponible = value;

                OnPropertyChanged(
                    nameof(disponible));

                NotificarComandos();
            }
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

        public RelayCommand ComandoCargar
        {
            get;
        }

        public MedicosViewModel(
            IMedicoService service)
        {
            _service = service;

            ComandoNuevo =
                new RelayCommand(
                    Nuevo,
                    () => disponible);

            ComandoRegistrar =
                new RelayCommand(
                    async () =>
                        await RegistrarAsync(),
                    () => disponible &&
                          medicoSeleccionado == null);

            ComandoActualizar =
                new RelayCommand(
                    async () =>
                        await ActualizarAsync(),
                    () => disponible &&
                          medicoSeleccionado != null);

            ComandoCargar =
                new RelayCommand(
                    async () =>
                        await CargarAsync(),
                    () => disponible);
        }

        public async Task CargarAsync()
        {
            await EjecutarAsync(async () =>
            {
                await CargarListaInternaAsync();

                LimpiarFormulario();

                mensaje =
                    "Médicos cargados correctamente.";
            });
        }

        private async Task RegistrarAsync()
        {
            await EjecutarAsync(async () =>
            {
                Medico medico = new()
                {
                    Codigo = codigo,
                    Nombre = nombre,
                    Especialidad = especialidad
                };

                int id =
                    await _service.RegistrarAsync(
                        medico);

                await CargarListaInternaAsync();

                LimpiarFormulario();

                mensaje =
                    $"Médico registrado correctamente. ID: {id}.";
            });
        }

        private async Task ActualizarAsync()
        {
            if (medicoSeleccionado == null)
            {
                mensaje =
                    "Seleccione un médico.";

                return;
            }

            int medicoId =
                medicoSeleccionado.Id;

            await EjecutarAsync(async () =>
            {
                Medico medico = new()
                {
                    Id = medicoId,
                    Codigo = codigo,
                    Nombre = nombre,
                    Especialidad = especialidad
                };

                await _service.ActualizarAsync(
                    medico);

                await CargarListaInternaAsync();

                LimpiarFormulario();

                mensaje =
                    "Médico actualizado correctamente.";
            });
        }

        private async Task CargarListaInternaAsync()
        {
            var lista =
                await _service.ListarAsync();

            medicos.Clear();

            foreach (Medico medico in lista)
            {
                medicos.Add(medico);
            }
        }

        private void Nuevo()
        {
            LimpiarFormulario();

            mensaje =
                "Complete los datos del nuevo médico.";
        }

        private void LimpiarFormulario()
        {
            medicoSeleccionado = null;
            codigo = string.Empty;
            nombre = string.Empty;
            especialidad = string.Empty;
        }

        private async Task EjecutarAsync(
            Func<Task> accion)
        {
            disponible = false;
            mensaje = "Procesando...";

            try
            {
                await accion();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            finally
            {
                disponible = true;
            }
        }

        private void NotificarComandos()
        {
            ComandoNuevo?
                .NotificarCanExecuteChanged();

            ComandoRegistrar?
                .NotificarCanExecuteChanged();

            ComandoActualizar?
                .NotificarCanExecuteChanged();

            ComandoCargar?
                .NotificarCanExecuteChanged();
        }
    }
}