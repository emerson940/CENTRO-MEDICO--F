using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
    }
}
