using System.Globalization;
using System.Text.RegularExpressions;

namespace SAT.API.Services
{
    public static class DateExtractor
    {
        private static readonly Regex RegexFecha = new Regex(
            @"(?<dia>\d{1,2})/(?<mes>\d{1,2})/(?<anio>\d{4})\s+(?<hora>\d{1,2}):(?<minuto>\d{2})",
            RegexOptions.Compiled);

        public static (DateTime? Fecha, bool EsValida) ExtraerFecha(string textoTiempo)
        {
            if (string.IsNullOrWhiteSpace(textoTiempo)) return (null, false);

            Match match = RegexFecha.Match(textoTiempo);
            if (!match.Success) return (null, false);

            int dia = int.Parse(match.Groups["dia"].Value);
            int mes = int.Parse(match.Groups["mes"].Value);
            int anio = int.Parse(match.Groups["anio"].Value);
            int hora = int.Parse(match.Groups["hora"].Value);
            int minuto = int.Parse(match.Groups["minuto"].Value);

            string fechaFormateada = $"{dia:D2}/{mes:D2}/{anio:D4} {hora:D2}:{minuto:D2}";

            if (DateTime.TryParseExact(fechaFormateada, "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaResult))
            {
                return (fechaResult, true);
            }

            return (null, false);
        }
    }
}