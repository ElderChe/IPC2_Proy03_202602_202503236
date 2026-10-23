using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace SAT.Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient cliente_http;
        private readonly string url_api = "https://localhost:7239/api/procesos";

        public IndexModel()
        {
            
            var manejador = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (mensaje, certificado, cadena, errores) => true
            };
            cliente_http = new HttpClient(manejador);
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCargarRtuAsync(IFormFile archivo_rtu)
        {
            if (archivo_rtu == null || archivo_rtu.Length == 0)
            {
                TempData["Mensaje"] = "No seleccionaste ningún archivo.";
                return Page();
            }

            using var lector = new StreamReader(archivo_rtu.OpenReadStream());
            string texto_json = await lector.ReadToEndAsync();

            var contenido = new StringContent(texto_json, Encoding.UTF8, "application/json");
            var respuesta = await cliente_http.PostAsync($"{url_api}/cargar-rtu", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                string resultado = await respuesta.Content.ReadAsStringAsync();
                TempData["Mensaje"] = $"RTU actualizado. Respuesta del servidor: {resultado}";
            }
            else
            {
                TempData["Mensaje"] = "Error al intentar enviar los datos a la API.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCargarFacturasAsync(IFormFile archivo_facturas)
        {
            if (archivo_facturas == null || archivo_facturas.Length == 0)
            {
                TempData["Mensaje"] = "No seleccionaste ningún archivo.";
                return Page();
            }

            using var lector = new StreamReader(archivo_facturas.OpenReadStream());
            string texto_json = await lector.ReadToEndAsync();

            var contenido = new StringContent(texto_json, Encoding.UTF8, "application/json");
            var respuesta = await cliente_http.PostAsync($"{url_api}/solicitar-autorizacion", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                string resultado = await respuesta.Content.ReadAsStringAsync();
                TempData["Mensaje"] = $"Facturas procesadas. Respuesta del servidor: {resultado}";
            }
            else
            {
                TempData["Mensaje"] = "Error al intentar enviar las facturas a la API.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostInicializarAsync()
        {
            var respuesta = await cliente_http.DeleteAsync($"{url_api}/limpiar-datos");

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "El sistema volvió a su estado inicial sin datos.";
            }
            else
            {
                TempData["Mensaje"] = "Hubo un error tratando de reiniciar el sistema.";
            }

            return RedirectToPage();
        }
    }
}