using Microsoft.AspNetCore.Mvc;
using SAT.API.Models;
using SAT.API.Services;

namespace SAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesosController : ControllerBase
    {
        private ManejoArchivos archivos_locales = new ManejoArchivos();

        
        [HttpPost("cargar-rtu")]
        public IActionResult CargarRtu([FromBody] List<Contribuyente> lista_entrante)
        {
            var rtu_guardado = archivos_locales.TraerRtu();
            var respuesta = new RtuResponse();

            foreach (var persona in lista_entrante)
            {
                if (!NitValidator.EsValido(persona.Nit))
                {
                    respuesta.NitsInvalidos++;
                    continue;
                }

                var persona_existente = rtu_guardado.FirstOrDefault(c => c.Nit == persona.Nit);

                if (persona_existente != null)
                {
                    persona_existente.Nombre = persona.Nombre;
                    respuesta.ContribuyentesActualizados++;
                }
                else
                {
                    rtu_guardado.Add(persona);
                    respuesta.ContribuyentesNuevos++;
                }
            }

            archivos_locales.GuardarRtuNuevo(rtu_guardado);
            return Ok(respuesta);
        }

        //  Recibe y procesa las solicitudes de factura
        [HttpPost("solicitar-autorizacion")]
        public IActionResult SolicitarAutorizacion([FromBody] List<FacturaRequest> facturas_entrantes)
        {
            var rtu = archivos_locales.TraerRtu();
            var facturas_registradas = archivos_locales.TraerFacturasGuardadas();
            var facturas_a_guardar = new List<FacturaProcesada>();

            var respuesta = new AutorizacionResponse();

            foreach (var factura in facturas_entrantes)
            {
                var factura_procesada = new FacturaProcesada
                {
                    FechaTexto = factura.Tiempo,
                    Ref = factura.Referencia,
                    Emisor = factura.NitEmisor,
                    Receptor = factura.NitReceptor,
                    MontoSinIva = factura.Valor,
                    MontoIva = factura.Iva,
                    TotalConIva = factura.Total,
                    TieneErrores = false
                };

                // La cosa del date extractor solo extrae la fecha y no lo demas 
                var extraccion_fecha = DateExtractor.ExtraerFecha(factura.Tiempo);
                if (extraccion_fecha.EsValida)
                {
                    factura_procesada.FechaHoraReal = extraccion_fecha.Fecha.Value;
                }
                else
                {
                    
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("FECHA con formato inválido");
                }

                // referencia si existo o non
                bool ref_duplicada = facturas_registradas.Any(f => f.Ref == factura.Referencia) ||
                                     facturas_a_guardar.Any(f => f.Ref == factura.Referencia);
                if (ref_duplicada)
                {
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("REFERENCIA duplicada");
                }

                //  esto de aca es para validar si el emisor existe en el RTU 
                if (!rtu.Any(c => c.Nit == factura.NitEmisor))
                {
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("NIT emisor inexistente");
                    respuesta.Resultado.RechazadasEmisorInexistente++;
                }

                
                if (!rtu.Any(c => c.Nit == factura.NitReceptor))
                {
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("NIT receptor inexistente");
                    respuesta.Resultado.RechazadasReceptorInexistente++;
                }

                
                decimal iva_calculado = Math.Round(factura.Valor * 0.12m, 2);
                if (factura.Iva != iva_calculado)
                {
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("IVA mal calculado");
                }

                decimal total_calculado = factura.Valor + factura.Iva;
                if (factura.Total != total_calculado)
                {
                    factura_procesada.TieneErrores = true;
                    factura_procesada.ListaDeErrores.Add("TOTAL mal calculado");
                }

                
                if (!factura_procesada.TieneErrores)
                {
                    factura_procesada.NumeroAutorizacion = archivos_locales.GenerarNumeroDeAutorizacion(factura_procesada.FechaHoraReal);

                    respuesta.Resultado.Aprobadas++;
                    respuesta.Resultado.DetalleAprobaciones.Add(new DetalleAprobacion
                    {
                        Referencia = factura_procesada.Ref,
                        CodigoAprobacion = factura_procesada.NumeroAutorizacion
                    });
                }

                facturas_a_guardar.Add(factura_procesada);
            }

            archivos_locales.GuardarFacturas(facturas_a_guardar);
            return Ok(respuesta);
        }

        
        [HttpDelete("limpiar-datos")]
        public IActionResult LimpiarDatos()
        {
            archivos_locales.BorrarTodo();
            return Ok(new { mensaje = "El sistema se devolvió a su estado inicial." });
        }
    }
}