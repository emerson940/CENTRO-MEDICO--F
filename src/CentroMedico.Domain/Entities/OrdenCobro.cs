using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class OrdenCobro
    {
        public int Id { get; set; }
        public int HistorialId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
    }
}
