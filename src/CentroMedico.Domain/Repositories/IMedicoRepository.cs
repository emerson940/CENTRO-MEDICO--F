using CentroMedico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Domain.Repositories
{
    public interface IMedicoRepository
    {
        Task<List<Medico>> ListarAsync();

        Task<int> RegistrarAsync(
            Medico medico);

        Task ActualizarAsync(
            Medico medico);
    }
}