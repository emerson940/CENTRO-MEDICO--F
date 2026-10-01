using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public class LoginRepositoryImpl : ILoginRepository
    {
        private readonly string cn;

        public LoginRepositoryImpl(string cadena)
        {
            cn = cadena;
        }

        public async Task<Usuario?> ValidarUsuarioAsync(
            string nombreUsuario,
            string clave)
        {
            using (SqlConnection conn =
                   new SqlConnection(cn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "sp_ValidarUsuario",
                           conn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@NombreUsuario",
                        SqlDbType.NVarChar,
                        40
                    ).Value = nombreUsuario;

                    cmd.Parameters.Add(
                        "@Clave",
                        SqlDbType.NVarChar,
                        100
                    ).Value = clave;

                    using (SqlDataReader reader =
                           await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Usuario usuario = new Usuario
                            {
                                Id = reader.GetInt32(0),

                                NombreUsuario =
                                    reader.GetString(1),

                                NombreCompleto =
                                    reader.GetString(2),

                                Rol =
                                    reader.GetString(3),

                                Activo =
                                    reader.GetBoolean(4)
                            };

                            return usuario;
                        }
                    }
                }
            }

            return null;
        }
    }
}