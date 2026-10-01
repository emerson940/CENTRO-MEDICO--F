using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;
namespace CentroMedico.UI.ViewModels
{
    public class RecetasViewModel : ListadoViewModel<Receta>
    {
        public RecetasViewModel(ICierreConsultaService servicio) : base(servicio.ListarRecetasAsync) { }
    }
}
