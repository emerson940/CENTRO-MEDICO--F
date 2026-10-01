using CentroMedico.Application.DTOs;
using CentroMedico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public interface ICierreConsultaService
    {
        Task<List<Cita>> ListarCitasPendientesAsync();
        Task<List<Insumo>> ListarInsumosAsync();
        Task<List<Historial>> ListarHistorialAsync();
        Task<List<Receta>> ListarRecetasAsync();
        Task<List<OrdenCobro>> ListarFacturasAsync();
        Task<OrdenCobro> CerrarConsultaAsync(CierreConsultaDTO datos);
    }
}
