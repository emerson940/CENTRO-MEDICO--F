using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;

namespace CentroMedico.UI.ViewModels
{
    public class HistorialViewModel
        : ListadoViewModel<Historial>
    {
        public HistorialViewModel(
            ICierreConsultaService servicio)
            : base(servicio.ListarHistorialAsync)
        {
        }
    }
}