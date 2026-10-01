using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Receta
    {
        public int Id { get; set; }
        public int HistorialId { get; set; }
        public DateTime Fecha { get; set; }
        public string Indicaciones { get; set; } = string.Empty;
        public List<DetalleReceta> Detalles { get; set; } = new List<DetalleReceta>();
    }
}
