using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Insumo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
