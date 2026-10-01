using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public class PacienteService
        : IPacienteService
    {
        private readonly IPacienteRepository
            _repository;

        public PacienteService(
            IPacienteRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Paciente>> ListarAsync()
        {
            return _repository.ListarAsync();
        }

        public Task<int> RegistrarAsync(
            Paciente paciente)
        {
            ValidarPaciente(paciente);

            paciente.Documento =
                paciente.Documento.Trim();

            paciente.Nombre =
                paciente.Nombre.Trim();

            paciente.Telefono =
                string.IsNullOrWhiteSpace(
                    paciente.Telefono)
                ? null
                : paciente.Telefono.Trim();

            return _repository.RegistrarAsync(
                paciente);
        }

        public Task ActualizarAsync(
            Paciente paciente)
        {
            if (paciente.Id <= 0)
            {
                throw new ArgumentException(
                    "Seleccione un paciente.");
            }

            ValidarPaciente(paciente);

            paciente.Documento =
                paciente.Documento.Trim();

            paciente.Nombre =
                paciente.Nombre.Trim();

            paciente.Telefono =
                string.IsNullOrWhiteSpace(
                    paciente.Telefono)
                ? null
                : paciente.Telefono.Trim();

            return _repository.ActualizarAsync(
                paciente);
        }

        private static void ValidarPaciente(
            Paciente paciente)
        {
            ArgumentNullException.ThrowIfNull(
                paciente);

            if (string.IsNullOrWhiteSpace(
                paciente.Documento))
            {
                throw new ArgumentException(
                    "Ingrese el documento.");
            }

            if (string.IsNullOrWhiteSpace(
                paciente.Nombre))
            {
                throw new ArgumentException(
                    "Ingrese el nombre.");
            }

            if (paciente.FechaNacimiento ==
                default)
            {
                throw new ArgumentException(
                    "Seleccione la fecha de nacimiento.");
            }

            if (paciente.FechaNacimiento.Date >
                DateTime.Today)
            {
                throw new ArgumentException(
                    "La fecha de nacimiento no puede ser futura.");
            }
        }
    }
}