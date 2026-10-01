using CentroMedico.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CentroMedico.Application.DTOs
{
    public class CierreConsultaDTO
    {
        public int CitaId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public string Diagnostico { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string Indicaciones { get; set; } = string.Empty;
        public List<DetalleReceta> Medicamentos { get; set; } = new List<DetalleReceta>();
        public List<ConsumoInsumo> Insumos { get; set; } = new List<ConsumoInsumo>();
    }
}
