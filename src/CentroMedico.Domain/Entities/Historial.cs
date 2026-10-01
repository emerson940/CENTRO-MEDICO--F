using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Historial
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
