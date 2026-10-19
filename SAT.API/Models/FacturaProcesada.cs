namespace SAT.API.Models
{
    public class FacturaProcesada
    {
        public DateTime FechaHoraReal { get; set; }
        public string FechaTexto { get; set; } = string.Empty;
        public string Ref { get; set; } = string.Empty;
        public string Emisor { get; set; } = string.Empty;
        public string Receptor { get; set; } = string.Empty;
        public decimal MontoSinIva { get; set; }
        public decimal MontoIva { get; set; }
        public decimal TotalConIva { get; set; }

        public long NumeroAutorizacion { get; set; }
        public bool TieneErrores { get; set; }
        public List<string> ListaDeErrores { get; set; } = new List<string>();
    }
}