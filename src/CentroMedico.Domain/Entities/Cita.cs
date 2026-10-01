using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Cita
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public Paciente Paciente { get; set; } = new Paciente();
        public Medico Medico { get; set; } = new Medico();
        public DateTime Fecha { get; set; }
        public decimal Tarifa { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
