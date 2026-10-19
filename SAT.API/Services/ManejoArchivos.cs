using System.Text.Json;
using SAT.API.Models;

namespace SAT.API.Services
{
    public class ManejoArchivos
    {
        private string archivo_rtu = "base_rtu.json";
        private string archivo_facturas = "registro_facturas.json";
        private string archivo_correlativo = "contador_diario.txt";

        public void GuardarRtuNuevo(List<Contribuyente> lista)
        {
            string texto_json = JsonSerializer.Serialize(lista);
            File.WriteAllText(archivo_rtu, texto_json);
        }

        public List<Contribuyente> TraerRtu()
        {
            if (!File.Exists(archivo_rtu))
            {
                return new List<Contribuyente>();
            }

            string texto_json = File.ReadAllText(archivo_rtu);
            return JsonSerializer.Deserialize<List<Contribuyente>>(texto_json) ?? new List<Contribuyente>();
        }

        public void GuardarFacturas(List<FacturaProcesada> nuevas_facturas)
        {
            List<FacturaProcesada> facturas_viejas = TraerFacturasGuardadas();
            facturas_viejas.AddRange(nuevas_facturas);

            string texto_json = JsonSerializer.Serialize(facturas_viejas);
            File.WriteAllText(archivo_facturas, texto_json);
        }

        public List<FacturaProcesada> TraerFacturasGuardadas()
        {
            if (!File.Exists(archivo_facturas))
            {
                return new List<FacturaProcesada>();
            }

            string texto_json = File.ReadAllText(archivo_facturas);
            return JsonSerializer.Deserialize<List<FacturaProcesada>>(texto_json) ?? new List<FacturaProcesada>();
        }

        public long GenerarNumeroDeAutorizacion(DateTime fecha_peticion)
        {
            string hoy_texto = fecha_peticion.ToString("yyyyMMdd");
            string fecha_guardada = "";
            int contador = 1;

            if (File.Exists(archivo_correlativo))
            {
                string[] lineas = File.ReadAllLines(archivo_correlativo);
                if (lineas.Length == 2)
                {
                    fecha_guardada = lineas[0];
                    contador = int.Parse(lineas[1]);
                }
            }

            if (hoy_texto == fecha_guardada)
            {
                contador++;
            }
            else
            {
                contador = 1; // Se resetea porque es un día nuevo
            }

            string ceros_relleno = contador.ToString().PadLeft(8, '0');
            string numero_final = $"{hoy_texto}{ceros_relleno}";

            // Actualizar el txt para la siguiente factura
            File.WriteAllLines(archivo_correlativo, new string[] { hoy_texto, contador.ToString() });

            return long.Parse(numero_final);
        }

        public void BorrarTodo()
        {
            if (File.Exists(archivo_rtu)) File.Delete(archivo_rtu);
            if (File.Exists(archivo_facturas)) File.Delete(archivo_facturas);
            if (File.Exists(archivo_correlativo)) File.Delete(archivo_correlativo);
        }
    }
}