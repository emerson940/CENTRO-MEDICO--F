using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.UI.Commands;
using System.Threading.Tasks;

namespace CentroMedico.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public GestionCitasViewModel GestionCitas { get; }
        public PacientesViewModel Pacientes { get; }
        public MedicosViewModel Medicos { get; }
        public CierreConsultaViewModel Cierre { get; }
        public HistorialViewModel Historial { get; }
        public RecetasViewModel Recetas { get; }
        public FacturacionViewModel Facturacion { get; }

        public RelayCommand ComandoGestionCitas { get; }
        public RelayCommand ComandoPacientes { get; }
        public RelayCommand ComandoMedicos { get; }
        public RelayCommand ComandoAgenda { get; }
        public RelayCommand ComandoHistorial { get; }
        public RelayCommand ComandoRecetas { get; }
        public RelayCommand ComandoFacturacion { get; }

        public string nombreUsuario { get; }
        public string rolUsuario { get; }

        public bool puedeGestionCitas { get; }
        public bool puedePacientes { get; }
        public bool puedeMedicos { get; }
        public bool puedeAgenda { get; }
        public bool puedeHistorial { get; }
        public bool puedeRecetas { get; }
        public bool puedeFacturacion { get; }

        private ViewModelBase _vistaActual;

        public ViewModelBase vistaActual
        {
            get => _vistaActual;
            set
            {
                _vistaActual = value;

                OnPropertyChanged(
                    nameof(vistaActual));
            }
        }

        public MainViewModel(
            ICierreConsultaService servicio,
            ICitaService citaService,
            PacientesViewModel pacientesViewModel,
            MedicosViewModel medicosViewModel,
            Usuario usuario)
        {
            nombreUsuario =
                usuario.NombreCompleto;

            rolUsuario =
                usuario.Rol;

            bool esAdministrador =
                rolUsuario == "ADMIN";

            bool esMedico =
                rolUsuario == "MEDICO";

            bool esRecepcion =
                rolUsuario == "RECEPCION";

            puedeGestionCitas =
                esAdministrador || esRecepcion;

            puedePacientes =
                esAdministrador || esRecepcion;

            puedeMedicos =
                esAdministrador;

            puedeAgenda =
                esAdministrador || esMedico;

            puedeHistorial =
                esAdministrador || esMedico;

            puedeRecetas =
                esAdministrador || esMedico;

            puedeFacturacion =
                esAdministrador || esRecepcion;

            GestionCitas =
                new GestionCitasViewModel(
                    citaService);

            Pacientes =
                pacientesViewModel;

            Medicos =
                medicosViewModel;

            Cierre =
                new CierreConsultaViewModel(
                    servicio);

            Historial =
                new HistorialViewModel(
                    servicio);

            Recetas =
                new RecetasViewModel(
                    servicio);

            Facturacion =
                new FacturacionViewModel(
                    servicio);

            if (esRecepcion)
            {
                _vistaActual =
                    GestionCitas;
            }
            else
            {
                _vistaActual =
                    Cierre;
            }

            ComandoGestionCitas =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            GestionCitas;

                        await GestionCitas
                            .CargarDatosAsync();
                    },
                    () => disponibleGeneral &&
                          puedeGestionCitas);

            ComandoPacientes =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            Pacientes;

                        await Pacientes
                            .CargarAsync();
                    },
                    () => disponibleGeneral &&
                          puedePacientes);

            ComandoMedicos =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            Medicos;

                        await Medicos
                            .CargarAsync();
                    },
                    () => disponibleGeneral &&
                          puedeMedicos);

            ComandoAgenda =
                new RelayCommand(
                    () => vistaActual = Cierre,
                    () => disponibleGeneral &&
                          puedeAgenda);

            ComandoHistorial =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            Historial;

                        await Historial
                            .CargarAsync();
                    },
                    () => disponibleGeneral &&
                          puedeHistorial);

            ComandoRecetas =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            Recetas;

                        await Recetas
                            .CargarAsync();
                    },
                    () => disponibleGeneral &&
                          puedeRecetas);

            ComandoFacturacion =
                new RelayCommand(
                    async () =>
                    {
                        vistaActual =
                            Facturacion;

                        await Facturacion
                            .CargarAsync();
                    },
                    () => disponibleGeneral &&
                          puedeFacturacion);

            Cierre.PropertyChanged +=
                (sender, e) =>
                {
                    if (e.PropertyName ==
                        nameof(Cierre.disponible))
                    {
                        NotificarComandos();
                    }
                };

            GestionCitas.PropertyChanged +=
                (sender, e) =>
                {
                    if (e.PropertyName ==
                        nameof(GestionCitas.disponible))
                    {
                        NotificarComandos();
                    }
                };

            

            Medicos.PropertyChanged +=
                (sender, e) =>
                {
                    if (e.PropertyName ==
                        nameof(Medicos.disponible))
                    {
                        NotificarComandos();
                    }
                };
        }

        private bool disponibleGeneral =>
            Cierre.disponible &&
            GestionCitas.disponible &&
            Medicos.disponible;

        public async Task CargarVistaInicialAsync()
        {
            if (rolUsuario == "RECEPCION")
            {
                await GestionCitas
                    .CargarDatosAsync();
            }
            else if (puedeAgenda)
            {
                await Cierre
                    .CargarDatosAsync();
            }
            else if (puedeFacturacion)
            {
                await Facturacion
                    .CargarAsync();
            }
        }

        private void NotificarComandos()
        {
            ComandoGestionCitas
                .NotificarCanExecuteChanged();

            ComandoPacientes
                .NotificarCanExecuteChanged();

            ComandoMedicos
                .NotificarCanExecuteChanged();

            ComandoAgenda
                .NotificarCanExecuteChanged();

            ComandoHistorial
                .NotificarCanExecuteChanged();

            ComandoRecetas
                .NotificarCanExecuteChanged();

            ComandoFacturacion
                .NotificarCanExecuteChanged();
        }
    }
}