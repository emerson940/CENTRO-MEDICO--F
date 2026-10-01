using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _repository;

        public MedicoService(
            IMedicoRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Medico>> ListarAsync()
        {
            return _repository.ListarAsync();
        }

        public Task<int> RegistrarAsync(
            Medico medico)
        {
            ValidarDatos(medico);

            return _repository.RegistrarAsync(medico);
        }

        public Task ActualizarAsync(
            Medico medico)
        {
            ArgumentNullException.ThrowIfNull(medico);

            if (medico.Id <= 0)
            {
                throw new ArgumentException(
                    "Seleccione un médico.");
            }

            ValidarDatos(medico);

            return _repository.ActualizarAsync(medico);
        }

        private static void ValidarDatos(
            Medico medico)
        {
            ArgumentNullException.ThrowIfNull(medico);

            medico.Codigo =
                medico.Codigo?.Trim() ?? string.Empty;

            medico.Nombre =
                medico.Nombre?.Trim() ?? string.Empty;

            medico.Especialidad =
                medico.Especialidad?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                medico.Codigo))
            {
                throw new ArgumentException(
                    "Ingrese el código del médico.");
            }

            if (medico.Codigo.Length > 12)
            {
                throw new ArgumentException(
                    "El código debe tener como máximo 12 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(
                medico.Nombre))
            {
                throw new ArgumentException(
                    "Ingrese el nombre del médico.");
            }

            if (medico.Nombre.Length > 100)
            {
                throw new ArgumentException(
                    "El nombre debe tener como máximo 100 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(
                medico.Especialidad))
            {
                throw new ArgumentException(
                    "Ingrese la especialidad del médico.");
            }

            if (medico.Especialidad.Length > 80)
            {
                throw new ArgumentException(
                    "La especialidad debe tener como máximo 80 caracteres.");
            }
        }
    }
}