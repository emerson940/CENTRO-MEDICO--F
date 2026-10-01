using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class ConsumoInsumo
    {
        public int HistorialId { get; set; }
        public int InsumoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
    }
}
