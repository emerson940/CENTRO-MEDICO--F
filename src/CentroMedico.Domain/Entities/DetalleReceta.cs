using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class DetalleReceta
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
    }
}
