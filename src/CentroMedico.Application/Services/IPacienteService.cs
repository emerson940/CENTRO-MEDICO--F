using CentroMedico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public interface IPacienteService
    {
        Task<List<Paciente>> ListarAsync();

        Task<int> RegistrarAsync(
            Paciente paciente);

        Task ActualizarAsync(
            Paciente paciente);
    }
}