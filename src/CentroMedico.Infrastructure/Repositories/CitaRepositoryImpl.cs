using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public class CitaRepositoryImpl : ICitaRepository
    {
        private readonly string cn;

        public CitaRepositoryImpl(string cadena)
        {
            cn = cadena;
        }

        public async Task<List<Paciente>> ListarPacientesAsync()
        {
            List<Paciente> lista = new List<Paciente>();

            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_ListarPacientes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader =
                           await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Paciente
                            {
                                Id = reader.GetInt32(0),
                                Documento = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                FechaNacimiento = reader.GetDateTime(3),
                                Telefono = reader.IsDBNull(4)
                                    ? null
                                    : reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<Medico>> ListarMedicosAsync()
        {
            List<Medico> lista = new List<Medico>();

            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_ListarMedicos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader =
                           await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Medico
                            {
                                Id = reader.GetInt32(0),
                                Codigo = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Especialidad = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<Cita>> ListarCitasAsync()
        {
            List<Cita> lista = new List<Cita>();

            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_ListarCitas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader =
                           await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Cita
                            {
                                Id = reader.GetInt32(0),
                                Codigo = reader.GetString(1),
                                Fecha = reader.GetDateTime(2),
                                Tarifa = reader.GetDecimal(3),
                                Estado = reader.GetString(4),
                                RowVersion = (byte[])reader[5],

                                Paciente = new Paciente
                                {
                                    Id = reader.GetInt32(6),
                                    Documento = reader.GetString(7),
                                    Nombre = reader.GetString(8)
                                },

                                Medico = new Medico
                                {
                                    Id = reader.GetInt32(9),
                                    Codigo = reader.GetString(10),
                                    Nombre = reader.GetString(11),
                                    Especialidad = reader.GetString(12)
                                }
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<int> RegistrarCitaAsync(Cita cita)
        {
            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_RegistrarCita", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@Codigo",
                        SqlDbType.VarChar,
                        25
                    ).Value = cita.Codigo;

                    cmd.Parameters.Add(
                        "@PacienteID",
                        SqlDbType.Int
                    ).Value = cita.Paciente.Id;

                    cmd.Parameters.Add(
                        "@MedicoID",
                        SqlDbType.Int
                    ).Value = cita.Medico.Id;

                    cmd.Parameters.Add(
                        "@Fecha",
                        SqlDbType.DateTime2
                    ).Value = cita.Fecha;

                    SqlParameter parametroTarifa =
                        cmd.Parameters.Add(
                            "@Tarifa",
                            SqlDbType.Decimal);

                    parametroTarifa.Precision = 12;
                    parametroTarifa.Scale = 2;
                    parametroTarifa.Value = cita.Tarifa;

                    object? resultado =
                        await cmd.ExecuteScalarAsync();

                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task ReprogramarCitaAsync(Cita cita)
        {
            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_ReprogramarCita", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@CitaID",
                        SqlDbType.Int
                    ).Value = cita.Id;

                    cmd.Parameters.Add(
                        "@MedicoID",
                        SqlDbType.Int
                    ).Value = cita.Medico.Id;

                    cmd.Parameters.Add(
                        "@Fecha",
                        SqlDbType.DateTime2
                    ).Value = cita.Fecha;

                    SqlParameter parametroTarifa =
                        cmd.Parameters.Add(
                            "@Tarifa",
                            SqlDbType.Decimal);

                    parametroTarifa.Precision = 12;
                    parametroTarifa.Scale = 2;
                    parametroTarifa.Value = cita.Tarifa;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CancelarCitaAsync(int citaId)
        {
            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand("sp_CancelarCita", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@CitaID",
                        SqlDbType.Int
                    ).Value = citaId;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}