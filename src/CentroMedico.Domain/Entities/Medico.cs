using System;
using System.Collections.Generic;

namespace CentroMedico.Domain.Entities
{
    public class Medico
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }
}
