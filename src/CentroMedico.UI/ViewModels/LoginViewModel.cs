using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
using CentroMedico.UI.Commands;
using System;

namespace CentroMedico.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILoginService _loginService;

        public RelayCommand ComandoIngresar { get; }

        public Action<Usuario>? OnLoginValido { get; set; }

        private string _nombreUsuario = string.Empty;

        public string nombreUsuario
        {
            get
            {
                return _nombreUsuario;
            }
            set
            {
                _nombreUsuario = value;
                OnPropertyChanged(nameof(nombreUsuario));
            }
        }

        private string _clave = string.Empty;

        public string clave
        {
            get
            {
                return _clave;
            }
            set
            {
                _clave = value;
                OnPropertyChanged(nameof(clave));
            }
        }

        private string _mensaje = string.Empty;

        public string mensaje
        {
            get
            {
                return _mensaje;
            }
            set
            {
                _mensaje = value;
                OnPropertyChanged(nameof(mensaje));
            }
        }

        private bool _ocupado;

        public bool ocupado
        {
            get
            {
                return _ocupado;
            }
            set
            {
                _ocupado = value;

                OnPropertyChanged(nameof(ocupado));
                OnPropertyChanged(nameof(disponible));

                ComandoIngresar
                    .NotificarCanExecuteChanged();
            }
        }

        public bool disponible
        {
            get
            {
                return !ocupado;
            }
        }

        public LoginViewModel(
            ILoginService loginService)
        {
            _loginService = loginService;

            ComandoIngresar =
                new RelayCommand(
                    IniciarSesion,
                    () => disponible);
        }

        private async void IniciarSesion()
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)
                || string.IsNullOrWhiteSpace(clave))
            {
                mensaje =
                    "Ingrese el usuario y la contraseña.";

                return;
            }

            ocupado = true;
            mensaje = "Validando usuario...";

            try
            {
                Usuario? usuarioValido =
                    await _loginService
                        .ValidarUsuarioAsync(
                            nombreUsuario,
                            clave);

                if (usuarioValido == null)
                {
                    mensaje =
                        "Usuario o contraseña incorrectos.";

                    return;
                }

                mensaje = string.Empty;

                OnLoginValido?.Invoke(usuarioValido);
            }
            catch (Exception ex)
            {
                mensaje =
                    "No se pudo iniciar sesión: "
                    + ex.Message;
            }
            finally
            {
                ocupado = false;
            }
        }
    }
}