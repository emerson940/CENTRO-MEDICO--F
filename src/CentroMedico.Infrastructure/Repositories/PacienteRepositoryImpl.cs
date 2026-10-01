using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public class PacienteRepositoryImpl
        : IPacienteRepository
    {
        private readonly string cn;

        public PacienteRepositoryImpl(
            string cadena)
        {
            cn = cadena;
        }

        public async Task<List<Paciente>>
            ListarAsync()
        {
            List<Paciente> lista = new();

            using SqlConnection conn = new(cn);

            using SqlCommand cmd = new(
                "sp_ListarPacientes",
                conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Paciente
                {
                    Id = reader.GetInt32(
                        reader.GetOrdinal("PacienteID")),

                    Documento = reader.GetString(
                        reader.GetOrdinal("Documento")),

                    Nombre = reader.GetString(
                        reader.GetOrdinal("Nombre")),

                    FechaNacimiento = reader.GetDateTime(
                        reader.GetOrdinal("FechaNacimiento")),

                    Telefono = reader.IsDBNull(
                        reader.GetOrdinal("Telefono"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Telefono"))
                });
            }

            return lista;
        }

        public async Task<int> RegistrarAsync(
            Paciente paciente)
        {
            using SqlConnection conn = new(cn);

            using SqlCommand cmd = new(
                "sp_RegistrarPaciente",
                conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Documento",
                SqlDbType.VarChar,
                12).Value = paciente.Documento;

            cmd.Parameters.Add(
                "@Nombre",
                SqlDbType.NVarChar,
                100).Value = paciente.Nombre;

            cmd.Parameters.Add(
                "@FechaNacimiento",
                SqlDbType.Date).Value =
                paciente.FechaNacimiento;

            cmd.Parameters.Add(
                "@Telefono",
                SqlDbType.NVarChar,
                20).Value =
                string.IsNullOrWhiteSpace(
                    paciente.Telefono)
                    ? DBNull.Value
                    : paciente.Telefono;

            await conn.OpenAsync();

            object? resultado =
                await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(resultado);
        }

        public async Task ActualizarAsync(
            Paciente paciente)
        {
            using SqlConnection conn = new(cn);

            using SqlCommand cmd = new(
                "sp_ActualizarPaciente",
                conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@PacienteID",
                SqlDbType.Int).Value =
                paciente.Id;

            cmd.Parameters.Add(
                "@Documento",
                SqlDbType.VarChar,
                12).Value =
                paciente.Documento;

            cmd.Parameters.Add(
                "@Nombre",
                SqlDbType.NVarChar,
                100).Value =
                paciente.Nombre;

            cmd.Parameters.Add(
                "@FechaNacimiento",
                SqlDbType.Date).Value =
                paciente.FechaNacimiento;

            cmd.Parameters.Add(
                "@Telefono",
                SqlDbType.NVarChar,
                20).Value =
                string.IsNullOrWhiteSpace(
                    paciente.Telefono)
                    ? DBNull.Value
                    : paciente.Telefono;

            await conn.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
        }
    }
}