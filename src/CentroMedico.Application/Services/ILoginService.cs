using CentroMedico.Domain.Entities;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public interface ILoginService
    {
        Task<Usuario?> ValidarUsuarioAsync(
            string nombreUsuario,
            string clave);
    }
}