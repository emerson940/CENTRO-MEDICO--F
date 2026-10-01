using CentroMedico.Application.DTOs;
using CentroMedico.Domain.Entities;
using CentroMedico.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroMedico.Application.Services
{
    public class CierreConsultaService : ICierreConsultaService
    {
        private readonly ICentroMedicoRepository _repository;

        public CierreConsultaService(ICentroMedicoRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Cita>> ListarCitasPendientesAsync() => _repository.ListarCitasPendientesAsync();
        public Task<List<Insumo>> ListarInsumosAsync() => _repository.ListarInsumosAsync();
        public Task<List<Historial>> ListarHistorialAsync() => _repository.ListarHistorialAsync();
        public Task<List<Receta>> ListarRecetasAsync() => _repository.ListarRecetasAsync();
        public Task<List<OrdenCobro>> ListarFacturasAsync() => _repository.ListarFacturasAsync();

        public Task<OrdenCobro> CerrarConsultaAsync(CierreConsultaDTO datos)
        {
            ArgumentNullException.ThrowIfNull(datos);
            CierreConsulta cierre = new CierreConsulta
            {
                CitaId = datos.CitaId,
                RowVersion = datos.RowVersion?.ToArray() ?? Array.Empty<byte>(),
                Diagnostico = datos.Diagnostico?.Trim() ?? string.Empty,
                Observaciones = string.IsNullOrWhiteSpace(datos.Observaciones) ? null : datos.Observaciones.Trim(),
                Indicaciones = datos.Indicaciones?.Trim() ?? string.Empty,
                Medicamentos = (datos.Medicamentos ?? new List<DetalleReceta>()).Select(detalle => new DetalleReceta
                {
                    Medicamento = detalle.Medicamento?.Trim() ?? string.Empty,
                    Dosis = detalle.Dosis?.Trim() ?? string.Empty,
                    Frecuencia = detalle.Frecuencia?.Trim() ?? string.Empty,
                    Duracion = detalle.Duracion?.Trim() ?? string.Empty
                }).ToList(),
                Insumos = (datos.Insumos ?? new List<ConsumoInsumo>()).Select(insumo => new ConsumoInsumo
                {
                    InsumoId = insumo.InsumoId,
                    Nombre = insumo.Nombre,
                    Cantidad = insumo.Cantidad
                }).ToList()
            };
            cierre.Validar();
            return _repository.CerrarConsultaAsync(cierre);
        }
    }
}
