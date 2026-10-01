using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CentroMedico.Infrastructure.Repositories
{
    public partial class CentroMedicoRepositoryImpl : ICentroMedicoRepository
    {
        private readonly string cn;

        public CentroMedicoRepositoryImpl(string cadena)
        {
            cn = cadena;
        }

        public async Task<OrdenCobro> CerrarConsultaAsync(CierreConsulta cierre)
        {
            cierre.Validar();

            using (SqlConnection conn = new SqlConnection(cn))
            {
                await conn.OpenAsync();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Mantiene la cita bloqueada hasta Commit/Rollback.
                        // RowVersion detecta si cambió desde que el usuario la cargó.
                        decimal tarifa;
                        string query = @"
                            SELECT Tarifa
                            FROM dbo.Citas WITH (UPDLOCK, HOLDLOCK)
                            WHERE CitaID = @CitaID AND Estado = 'PENDIENTE'
                              AND RowVersion = @RowVersion;";
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.Add("@CitaID", SqlDbType.Int).Value = cierre.CitaId;
                            cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Value = cierre.RowVersion;
                            object? valor = await cmd.ExecuteScalarAsync();
                            if (valor == null || valor == DBNull.Value)
                                throw new InvalidOperationException("La cita no existe, ya fue cerrada o cambió. Recargue la agenda.");
                            tarifa = Convert.ToDecimal(valor);
                        }

                        // 1. Registrar diagnóstico e historial.
                        int idHistorial;
                        query = @"
                            INSERT INTO dbo.Historial(CitaID, Diagnostico, Observaciones)
                            OUTPUT INSERTED.HistorialID
                            VALUES (@CitaID, @Diagnostico, @Observaciones);";
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.Add("@CitaID", SqlDbType.Int).Value = cierre.CitaId;
                            cmd.Parameters.Add("@Diagnostico", SqlDbType.NVarChar, 500).Value = cierre.Diagnostico;
                            cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 1000).Value =
                                (object?)cierre.Observaciones ?? DBNull.Value;
                            idHistorial = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }

                        // 2. Generar receta y todos sus medicamentos.
                        int idReceta;
                        query = @"
                            INSERT INTO dbo.Recetas(HistorialID, Indicaciones)
                            OUTPUT INSERTED.RecetaID VALUES (@HistorialID, @Indicaciones);";
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.Add("@HistorialID", SqlDbType.Int).Value = idHistorial;
                            cmd.Parameters.Add("@Indicaciones", SqlDbType.NVarChar, 1000).Value = cierre.Indicaciones;
                            idReceta = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }
                        foreach (DetalleReceta detalle in cierre.Medicamentos)
                        {
                            query = @"
                                INSERT INTO dbo.DetalleReceta(RecetaID, Medicamento, Dosis, Frecuencia, Duracion)
                                VALUES (@RecetaID, @Medicamento, @Dosis, @Frecuencia, @Duracion);";
                            using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                            {
                                cmd.Parameters.Add("@RecetaID", SqlDbType.Int).Value = idReceta;
                                cmd.Parameters.Add("@Medicamento", SqlDbType.NVarChar, 100).Value = detalle.Medicamento;
                                cmd.Parameters.Add("@Dosis", SqlDbType.NVarChar, 100).Value = detalle.Dosis;
                                cmd.Parameters.Add("@Frecuencia", SqlDbType.NVarChar, 100).Value = detalle.Frecuencia;
                                cmd.Parameters.Add("@Duracion", SqlDbType.NVarChar, 100).Value = detalle.Duracion;
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // 3. Emitir orden de cobro: todavía no registra un pago.
                        OrdenCobro orden;
                        query = @"
                            INSERT INTO dbo.Facturas(HistorialID, Total)
                            OUTPUT INSERTED.FacturaID, INSERTED.HistorialID,
                                   INSERTED.Fecha, INSERTED.Total, INSERTED.Estado
                            VALUES (@HistorialID, @Total);";
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.Add("@HistorialID", SqlDbType.Int).Value = idHistorial;
                            AgregarDecimal(cmd, "@Total", tarifa);
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (!await reader.ReadAsync())
                                    throw new InvalidOperationException("No se generó la orden de cobro.");
                                orden = new OrdenCobro
                                {
                                    Id = reader.GetInt32(0), HistorialId = reader.GetInt32(1),
                                    Fecha = reader.GetDateTime(2), Total = reader.GetDecimal(3),
                                    Estado = reader.GetString(4)
                                };
                            }
                        }

                        // 4. Descontar insumos y registrar su consumo.
                        // Orden estable para reducir conflictos entre cierres simultáneos.
                        foreach (ConsumoInsumo insumo in cierre.Insumos.OrderBy(x => x.InsumoId))
                        {
                            query = @"
                                UPDATE dbo.Insumos SET Stock = Stock - @Cantidad
                                OUTPUT INSERTED.InsumoID
                                WHERE InsumoID = @InsumoID AND Stock >= @Cantidad;";
                            using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                            {
                                cmd.Parameters.Add("@InsumoID", SqlDbType.Int).Value = insumo.InsumoId;
                                AgregarDecimal(cmd, "@Cantidad", insumo.Cantidad);
                                object? valor = await cmd.ExecuteScalarAsync();
                                if (valor == null)
                                    throw new InvalidOperationException(
                                        $"Stock insuficiente o insumo inexistente: {insumo.Nombre} (ID {insumo.InsumoId}).");
                            }
                            query = @"
                                INSERT INTO dbo.ConsumoInsumos(HistorialID, InsumoID, Cantidad)
                                VALUES (@HistorialID, @InsumoID, @Cantidad);";
                            using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                            {
                                cmd.Parameters.Add("@HistorialID", SqlDbType.Int).Value = idHistorial;
                                cmd.Parameters.Add("@InsumoID", SqlDbType.Int).Value = insumo.InsumoId;
                                AgregarDecimal(cmd, "@Cantidad", insumo.Cantidad);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        query = "UPDATE dbo.Citas SET Estado = 'CERRADA' WHERE CitaID = @CitaID;";
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.Add("@CitaID", SqlDbType.Int).Value = cierre.CitaId;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        tx.Commit();
                        return orden;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            tx.Rollback();
                        }
                        catch (Exception errorRollback)
                        {
                            // Una conexión perdida no permite afirmar el resultado del Commit.
                            throw new AggregateException(
                                "No se pudo confirmar el resultado. Recargue y verifique la cita antes de reintentar.",
                                ex, errorRollback);
                        }
                        throw;
                    }
                }
            }
        }

        private static void AgregarDecimal(SqlCommand cmd, string nombre, decimal valor)
        {
            SqlParameter parametro = cmd.Parameters.Add(nombre, SqlDbType.Decimal);
            parametro.Precision = 12;
            parametro.Scale = 2;
            parametro.Value = valor;
        }
    }
}
