using CentroMedico.Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public partial class CentroMedicoRepositoryImpl
    {
        public async Task<List<Cita>> ListarCitasPendientesAsync()
        {
            List<Cita> lista = new List<Cita>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                string query = @"SELECT C.CitaID, C.Codigo, C.Fecha, C.Tarifa, C.Estado, C.RowVersion,
                                   P.PacienteID, P.Nombre, M.MedicoID, M.Nombre
                            FROM dbo.Citas C
                            INNER JOIN dbo.Pacientes P ON P.PacienteID = C.PacienteID
                            INNER JOIN dbo.Medicos M ON M.MedicoID = C.MedicoID
                            WHERE C.Estado = 'PENDIENTE' ORDER BY C.Fecha, C.CitaID;";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Cita
                        {
                                Id = reader.GetInt32(0), Codigo = reader.GetString(1),
                                Fecha = reader.GetDateTime(2), Tarifa = reader.GetDecimal(3),
                                Estado = reader.GetString(4), RowVersion = (byte[])reader[5],
                                Paciente = new Paciente { Id = reader.GetInt32(6), Nombre = reader.GetString(7) },
                                Medico = new Medico { Id = reader.GetInt32(8), Nombre = reader.GetString(9) }
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<Insumo>> ListarInsumosAsync()
        {
            List<Insumo> lista = new List<Insumo>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                string query = @"SELECT InsumoID, Nombre, Unidad, Stock, RowVersion FROM dbo.Insumos ORDER BY InsumoID;";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Insumo
                        {
                                Id = reader.GetInt32(0), Nombre = reader.GetString(1), Unidad = reader.GetString(2),
                                Stock = reader.GetDecimal(3), RowVersion = (byte[])reader[4]
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<Historial>> ListarHistorialAsync()
        {
            List<Historial> lista = new List<Historial>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                string query = @"SELECT HistorialID, CitaID, Fecha, Diagnostico, Observaciones FROM dbo.Historial ORDER BY HistorialID DESC;";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Historial
                        {
                                Id = reader.GetInt32(0), CitaId = reader.GetInt32(1), Fecha = reader.GetDateTime(2),
                                Diagnostico = reader.GetString(3), Observaciones = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<OrdenCobro>> ListarFacturasAsync()
        {
            List<OrdenCobro> lista = new List<OrdenCobro>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                string query = @"SELECT FacturaID, HistorialID, Fecha, Total, Estado FROM dbo.Facturas ORDER BY FacturaID DESC;";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new OrdenCobro
                        {
                                Id = reader.GetInt32(0), HistorialId = reader.GetInt32(1), Fecha = reader.GetDateTime(2),
                                Total = reader.GetDecimal(3), Estado = reader.GetString(4)
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<Receta>> ListarRecetasAsync()
        {
            List<Receta> lista = new List<Receta>();
            Dictionary<int, Receta> recetas = new Dictionary<int, Receta>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                string query = @"SELECT R.RecetaID, R.HistorialID, R.Fecha, R.Indicaciones,
                                       D.DetalleID, D.Medicamento, D.Dosis, D.Frecuencia, D.Duracion
                                FROM dbo.Recetas R
                                LEFT JOIN dbo.DetalleReceta D ON D.RecetaID = R.RecetaID
                                ORDER BY R.RecetaID DESC, D.DetalleID;";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(0);
                        if (!recetas.TryGetValue(id, out Receta? receta))
                        {
                            receta = new Receta
                            {
                                Id = id, HistorialId = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2), Indicaciones = reader.GetString(3)
                            };
                            recetas.Add(id, receta);
                            lista.Add(receta);
                        }
                        if (!reader.IsDBNull(4))
                        {
                            receta.Detalles.Add(new DetalleReceta
                            {
                                Id = reader.GetInt32(4), RecetaId = id, Medicamento = reader.GetString(5),
                                Dosis = reader.GetString(6), Frecuencia = reader.GetString(7), Duracion = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
