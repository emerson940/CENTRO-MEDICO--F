using CentroMedico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public interface ICitaService
    {
        Task<List<Paciente>> ListarPacientesAsync();

        Task<List<Medico>> ListarMedicosAsync();

        Task<List<Cita>> ListarCitasAsync();

        Task<int> RegistrarCitaAsync(Cita cita);

        Task ReprogramarCitaAsync(Cita cita);

        Task CancelarCitaAsync(int citaId);
    }
}