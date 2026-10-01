using CentroMedico.Application.Services;
using CentroMedico.Domain.Entities;

namespace CentroMedico.UI.ViewModels
{
    public class FacturacionViewModel
        : ListadoViewModel<OrdenCobro>
    {
        public FacturacionViewModel(
            ICierreConsultaService servicio)
            : base(servicio.ListarFacturasAsync)
        {
        }
    }
}