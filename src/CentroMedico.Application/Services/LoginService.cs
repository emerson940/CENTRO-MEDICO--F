using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;

        public LoginService(
            ILoginRepository repository)
        {
            _repository = repository;
        }

        public Task<Usuario?> ValidarUsuarioAsync(
            string nombreUsuario,
            string clave)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)
                || string.IsNullOrWhiteSpace(clave))
            {
                return Task.FromResult<Usuario?>(null);
            }

            return _repository.ValidarUsuarioAsync(
                nombreUsuario.Trim(),
                clave);
        }
    }
}