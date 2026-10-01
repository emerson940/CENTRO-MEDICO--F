using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _repository;

        public CitaService(ICitaRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Paciente>> ListarPacientesAsync()
        {
            return _repository.ListarPacientesAsync();
        }

        public Task<List<Medico>> ListarMedicosAsync()
        {
            return _repository.ListarMedicosAsync();
        }

        public Task<List<Cita>> ListarCitasAsync()
        {
            return _repository.ListarCitasAsync();
        }

        public Task<int> RegistrarCitaAsync(Cita cita)
        {
            ArgumentNullException.ThrowIfNull(cita);

            if (string.IsNullOrWhiteSpace(cita.Codigo))
                throw new ArgumentException(
                    "Ingrese el código de la cita.");

            if (cita.Paciente == null || cita.Paciente.Id <= 0)
                throw new ArgumentException(
                    "Seleccione un paciente.");

            if (cita.Medico == null || cita.Medico.Id <= 0)
                throw new ArgumentException(
                    "Seleccione un médico.");

            if (cita.Tarifa <= 0)
                throw new ArgumentException(
                    "La tarifa debe ser mayor que cero.");

            cita.Codigo = cita.Codigo.Trim();

            return _repository.RegistrarCitaAsync(cita);
        }

        public Task ReprogramarCitaAsync(Cita cita)
        {
            ArgumentNullException.ThrowIfNull(cita);

            if (cita.Id <= 0)
                throw new ArgumentException(
                    "Seleccione una cita.");

            if (cita.Medico == null || cita.Medico.Id <= 0)
                throw new ArgumentException(
                    "Seleccione un médico.");

            if (cita.Tarifa <= 0)
                throw new ArgumentException(
                    "La tarifa debe ser mayor que cero.");

            return _repository.ReprogramarCitaAsync(cita);
        }

        public Task CancelarCitaAsync(int citaId)
        {
            if (citaId <= 0)
                throw new ArgumentException(
                    "Seleccione una cita.");

            return _repository.CancelarCitaAsync(citaId);
        }
    }
}