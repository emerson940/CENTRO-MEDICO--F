using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public class MedicoRepositoryImpl : IMedicoRepository
    {
        private readonly string _connectionString;

        public MedicoRepositoryImpl(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Medico>> ListarAsync()
        {
            List<Medico> lista = new();

            using SqlConnection conn =
                new(_connectionString);

            using SqlCommand cmd =
                new("sp_ListarMedicos", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Medico
                {
                    Id = reader.GetInt32(
                        reader.GetOrdinal("MedicoID")),

                    Codigo = reader.GetString(
                        reader.GetOrdinal("Codigo")),

                    Nombre = reader.GetString(
                        reader.GetOrdinal("Nombre")),

                    Especialidad = reader.GetString(
                        reader.GetOrdinal("Especialidad"))
                });
            }

            return lista;
        }

        public async Task<int> RegistrarAsync(
            Medico medico)
        {
            using SqlConnection conn =
                new(_connectionString);

            using SqlCommand cmd =
                new("sp_RegistrarMedico", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Codigo",
                SqlDbType.VarChar,
                12
            ).Value = medico.Codigo;

            cmd.Parameters.Add(
                "@Nombre",
                SqlDbType.NVarChar,
                100
            ).Value = medico.Nombre;

            cmd.Parameters.Add(
                "@Especialidad",
                SqlDbType.NVarChar,
                80
            ).Value = medico.Especialidad;

            await conn.OpenAsync();

            object? resultado =
                await cmd.ExecuteScalarAsync();

            if (resultado == null ||
                resultado == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "No se pudo obtener el ID del médico.");
            }

            return Convert.ToInt32(resultado);
        }

        public async Task ActualizarAsync(
            Medico medico)
        {
            using SqlConnection conn =
                new(_connectionString);

            using SqlCommand cmd =
                new("sp_ActualizarMedico", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@MedicoID",
                SqlDbType.Int
            ).Value = medico.Id;

            cmd.Parameters.Add(
                "@Codigo",
                SqlDbType.VarChar,
                12
            ).Value = medico.Codigo;

            cmd.Parameters.Add(
                "@Nombre",
                SqlDbType.NVarChar,
                100
            ).Value = medico.Nombre;

            cmd.Parameters.Add(
                "@Especialidad",
                SqlDbType.NVarChar,
                80
            ).Value = medico.Especialidad;

            await conn.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
        }
    }
}