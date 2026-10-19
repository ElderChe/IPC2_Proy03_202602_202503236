using System.Text.Json.Serialization;

namespace SAT.API.Models
{
    public class FacturaRequest
    {
        [JsonPropertyName("TIEMPO")]
        public string Tiempo { get; set; } = string.Empty;

        [JsonPropertyName("REFERENCIA")]
        public string Referencia { get; set; } = string.Empty;

        [JsonPropertyName("NIT_EMISOR")]
        public string NitEmisor { get; set; } = string.Empty;

        [JsonPropertyName("NIT_RECEPTOR")]
        public string NitReceptor { get; set; } = string.Empty;

        [JsonPropertyName("VALOR")]
        public decimal Valor { get; set; }

        [JsonPropertyName("IVA")]
        public decimal Iva { get; set; }

        [JsonPropertyName("TOTAL")]
        public decimal Total { get; set; }
    }
}