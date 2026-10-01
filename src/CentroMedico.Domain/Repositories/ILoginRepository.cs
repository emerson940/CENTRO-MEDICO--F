using CentroMedico.Domain.Entities;
using System.Threading.Tasks;

namespace CentroMedico.Domain.Repositories
{
    public interface ILoginRepository
    {
        Task<Usuario?> ValidarUsuarioAsync(
            string nombreUsuario,
            string clave);
    }
}