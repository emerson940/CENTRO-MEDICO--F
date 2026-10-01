using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using CentroMedico.Infrastructure.Repositories;
using CentroMedico.UI.ViewModels;
using System;
using System.Configuration;
using System.Windows;

namespace CentroMedico.UI
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string cn = ConfigurationManager .ConnectionStrings[ "CentroMedico.UI.Properties.Settings.CentroMedico" ]?.ConnectionString
                    ?? throw new InvalidOperationException(
                        "Configure la conexión en App.config.");

                ILoginRepository loginRepository =
                    new LoginRepositoryImpl(cn);

                ILoginService loginService =
                    new LoginService(loginRepository);

                LoginViewModel loginViewModel =
                    new LoginViewModel(loginService);

                Window loginWindow =
                    (Window)LoadComponent(
                        new Uri(
                            "Views/LoginWindow.xaml",
                            UriKind.Relative));

                loginWindow.DataContext =
                    loginViewModel;

                loginViewModel.OnLoginValido =
                    usuario =>
                    AbrirSistema(
                        usuario,
                        loginWindow,
                        cn);

                MainWindow = loginWindow;

                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo iniciar: "
                    + ex.Message);

                Shutdown();
            }
        }

        private async void AbrirSistema( Usuario usuario,Window loginWindow,string cn)
        {
            try
            {
                ICentroMedicoRepository repository = new CentroMedicoRepositoryImpl(cn);

                ICierreConsultaService servicio =new CierreConsultaService(repository);

                ICitaRepository citaRepository = new CitaRepositoryImpl(cn);

                ICitaService citaService =new CitaService(citaRepository);

                IPacienteRepository pacienteRepository =new PacienteRepositoryImpl(cn);

                IPacienteService pacienteService = new PacienteService( pacienteRepository);

                PacientesViewModel pacientesViewModel =new PacientesViewModel( pacienteService);

                IMedicoRepository medicoRepository =  new MedicoRepositoryImpl(cn);

                IMedicoService medicoService = new MedicoService(medicoRepository);

                MedicosViewModel medicosViewModel = new MedicosViewModel(medicoService);

                MainViewModel viewModel =
                      new MainViewModel(
                           servicio,
                          citaService,
                          pacientesViewModel,
                           medicosViewModel,
                           usuario);

                Window ventanaPrincipal =
                    (Window)LoadComponent(
                        new Uri(
                            "Views/MainWindow.xaml",
                            UriKind.Relative));

                ventanaPrincipal.DataContext =
                    viewModel;

                ventanaPrincipal.Title =
                    "Centro Médico - "
                    + usuario.NombreCompleto
                    + " (" + usuario.Rol + ")";

                MainWindow = ventanaPrincipal;

                ventanaPrincipal.Show();

                loginWindow.Close();

                await viewModel .CargarVistaInicialAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo abrir el sistema: "
                    + ex.Message);
            }
        }
    }
}