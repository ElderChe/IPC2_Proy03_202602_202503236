namespace SAT.API.Services
{
    public static class NitValidator
    {
        public static bool EsValido(string nit)
        {
            if (string.IsNullOrWhiteSpace(nit)) return false;
            nit = nit.Trim().ToUpper();
            if (nit.Length < 2) return false;

            char verificadorEsperado = nit[^1];
            string cuerpo = nit[..^1];

            if (!long.TryParse(cuerpo, out _)) return false;

            int suma = 0;
            int posicion = 2; 

            for (int i = cuerpo.Length - 1; i >= 0; i--)
            {
                suma += (cuerpo[i] - '0') * posicion;
                posicion++;
            }

            int mod11Suma = suma % 11;
            int resta = 11 - mod11Suma;
            int residuoFinal = resta % 11;

            char verificadorCalculado = residuoFinal == 10 ? 'K' : residuoFinal.ToString()[0];

            return verificadorEsperado == verificadorCalculado;
        }
    }
}