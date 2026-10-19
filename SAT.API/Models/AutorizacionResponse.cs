using System.Text.Json.Serialization;

namespace SAT.API.Models
{
    public class AutorizacionResponse
    {
        [JsonPropertyName("resultadoAutorizaciones")]
        public ResultadoAutorizaciones Resultado { get; set; } = new ResultadoAutorizaciones();
    }

    public class ResultadoAutorizaciones
    {
        [JsonPropertyName("cantidadAutorizacionesAprobadas")]
        public int Aprobadas { get; set; }

        [JsonPropertyName("cantidadAutorizacionesRechazadasPorNitEmisorInexistente")]
        public int RechazadasEmisorInexistente { get; set; }

        [JsonPropertyName("cantidadAutorizacionesRechazadasPorNitReceptorInexistente")]
        public int RechazadasReceptorInexistente { get; set; }

        [JsonPropertyName("detalleAprobaciones")]
        public List<DetalleAprobacion> DetalleAprobaciones { get; set; } = new List<DetalleAprobacion>();
    }

    public class DetalleAprobacion
    {
        [JsonPropertyName("referencia")]
        public string Referencia { get; set; } = string.Empty;

        [JsonPropertyName("codigoAprobacion")]
        public long CodigoAprobacion { get; set; }
    }
}