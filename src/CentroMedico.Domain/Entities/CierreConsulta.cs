using System;
using System.Collections.Generic;
using System.Linq;

namespace CentroMedico.Domain.Entities
{
    public class CierreConsulta
    {
        public int CitaId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public string Diagnostico { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string Indicaciones { get; set; } = string.Empty;
        public List<DetalleReceta> Medicamentos { get; set; } = new List<DetalleReceta>();
        public List<ConsumoInsumo> Insumos { get; set; } = new List<ConsumoInsumo>();

        public void Validar()
        {
            if (CitaId <= 0 || RowVersion == null || RowVersion.Length != 8)
                throw new ArgumentException("Seleccione una cita y vuelva a cargarla si fue modificada.");
            if (string.IsNullOrWhiteSpace(Diagnostico) || Diagnostico.Length > 500)
                throw new ArgumentException("El diagnóstico es obligatorio y admite hasta 500 caracteres.");
            if (Observaciones?.Length > 1000)
                throw new ArgumentException("Las observaciones admiten hasta 1000 caracteres.");
            if (string.IsNullOrWhiteSpace(Indicaciones) || Indicaciones.Length > 1000)
                throw new ArgumentException("Las indicaciones son obligatorias y admiten hasta 1000 caracteres.");
            if (Medicamentos == null || Medicamentos.Count == 0)
                throw new ArgumentException("Agregue al menos un medicamento a la receta.");
            foreach (DetalleReceta detalle in Medicamentos)
            {
                if (detalle == null ||
                    string.IsNullOrWhiteSpace(detalle.Medicamento) || detalle.Medicamento.Length > 100 ||
                    string.IsNullOrWhiteSpace(detalle.Dosis) || detalle.Dosis.Length > 100 ||
                    string.IsNullOrWhiteSpace(detalle.Frecuencia) || detalle.Frecuencia.Length > 100 ||
                    string.IsNullOrWhiteSpace(detalle.Duracion) || detalle.Duracion.Length > 100)
                    throw new ArgumentException("Complete medicamento, dosis, frecuencia y duración (máximo 100 caracteres por campo).");
            }
            if (Insumos == null || Insumos.Count == 0)
                throw new ArgumentException("Agregue los insumos utilizados en la consulta.");
            foreach (ConsumoInsumo insumo in Insumos)
            {
                if (insumo == null || insumo.InsumoId <= 0 || insumo.Cantidad <= 0 ||
                    insumo.Cantidad > 9999999999.99m || decimal.Round(insumo.Cantidad, 2) != insumo.Cantidad)
                    throw new ArgumentException("Las cantidades de insumos deben ser positivas y tener como máximo dos decimales.");
            }
            if (Insumos.Select(x => x.InsumoId).Distinct().Count() != Insumos.Count)
                throw new ArgumentException("Cada insumo debe aparecer una sola vez con su cantidad total.");
        }
    }
}
