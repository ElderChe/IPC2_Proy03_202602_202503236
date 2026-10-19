using System.Text.Json.Serialization;

namespace SAT.API.Models
{
    public class Contribuyente
    {
        [JsonPropertyName("NIT")]
        public string Nit { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}