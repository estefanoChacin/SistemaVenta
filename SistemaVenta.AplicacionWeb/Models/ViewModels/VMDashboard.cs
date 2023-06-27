namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMDashboard
    {
        public int TotalVenta { get; set; }
        public string? TotalIngresos { get; set; }
        public string? TotalProductos { get; set; }
        public string? TotalCategorias { get; set; }

        public List<VMVentasSemana> VentasUltimaSemana { get;set; }
        public List<VMProdcutosSemanas> ProductosTopUltimaSemana { get; set; }



    }
}
