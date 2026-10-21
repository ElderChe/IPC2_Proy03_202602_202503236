using Microsoft.AspNetCore.Mvc;
using SAT.API.Models;
using SAT.API.Services;

namespace SAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private ManejoArchivos control_archivos = new ManejoArchivos();

        [HttpGet("general-errores")]
        public IActionResult SacarReporteErrores(string inicio, string fin)
        {
            var todas_las_facturas = control_archivos.TraerFacturasGuardadas();
            DateTime fecha1 = DateTime.Parse(inicio);
            DateTime fecha2 = DateTime.Parse(fin);

            var facturas_en_rango = todas_las_facturas
                .Where(f => f.FechaHoraReal.Date >= fecha1.Date && f.FechaHoraReal.Date <= fecha2.Date)
                .ToList();

            var agrupado_por_dia = facturas_en_rango.GroupBy(f => f.FechaHoraReal.Date).Select(grupo => new
            {
                Fecha = grupo.Key.ToString("dd/MM/yyyy"),
                TotalRecibidas = grupo.Count(),
                ErroresEmisorInvalido = grupo.Count(f => f.ListaDeErrores.Contains("NIT emisor inválido")),
                ErroresReceptorInvalido = grupo.Count(f => f.ListaDeErrores.Contains("NIT receptor inválido")),
                ErroresEmisorNoExiste = grupo.Count(f => f.ListaDeErrores.Contains("NIT emisor inexistente")),
                ErroresReceptorNoExiste = grupo.Count(f => f.ListaDeErrores.Contains("NIT receptor inexistente")),
                ErroresIva = grupo.Count(f => f.ListaDeErrores.Contains("IVA mal calculado")),
                ErroresTotal = grupo.Count(f => f.ListaDeErrores.Contains("TOTAL mal calculado")),
                ErroresRefDuplicada = grupo.Count(f => f.ListaDeErrores.Contains("REFERENCIA duplicada")),
                FacturasLimpias = grupo.Count(f => !f.TieneErrores),
                EmisoresDistintos = grupo.Where(f => !f.TieneErrores).Select(f => f.Emisor).Distinct().Count(),
                ReceptoresDistintos = grupo.Where(f => !f.TieneErrores).Select(f => f.Receptor).Distinct().Count()
            }).ToList();

            return Ok(agrupado_por_dia);
        }

        [HttpGet("por-contribuyente")]
        public IActionResult PorContribuyente(string nit, string tipo, string inicio, string fin)
        {
            var facturas = control_archivos.TraerFacturasGuardadas();
            DateTime fecha1 = DateTime.Parse(inicio);
            DateTime fecha2 = DateTime.Parse(fin);

            var filtro_fecha = facturas.Where(f => !f.TieneErrores && f.FechaHoraReal.Date >= fecha1.Date && f.FechaHoraReal.Date <= fecha2.Date);

            var filtro_final = tipo.ToLower() == "emitidas"
                ? filtro_fecha.Where(f => f.Emisor == nit)
                : filtro_fecha.Where(f => f.Receptor == nit);

            var reporte_dias = filtro_final.GroupBy(f => f.FechaHoraReal.Date).Select(g => new
            {
                Fecha = g.Key.ToString("dd/MM/yyyy"),
                Cantidad = g.Count(),
                Monto = g.Sum(x => x.TotalConIva)
            }).ToList();

            var total_rango = filtro_final.Sum(f => f.TotalConIva);

            return Ok(new { PorDia = reporte_dias, GranTotal = total_rango });
        }

        [HttpGet("resumen-iva")]
        public IActionResult ResumenIva(string fecha_pedida)
        {
            DateTime dia = DateTime.Parse(fecha_pedida);
            var facturas_dia = control_archivos.TraerFacturasGuardadas()
                .Where(f => !f.TieneErrores && f.FechaHoraReal.Date == dia.Date)
                .ToList();

            var nits_involucrados = facturas_dia.Select(f => f.Emisor)
                .Union(facturas_dia.Select(f => f.Receptor))
                .Distinct();

            var lista_nits = new List<object>();

            foreach (var un_nit in nits_involucrados)
            {
                decimal iva_dado = facturas_dia.Where(f => f.Emisor == un_nit).Sum(f => f.MontoIva);
                decimal iva_cobrado = facturas_dia.Where(f => f.Receptor == un_nit).Sum(f => f.MontoIva);

                lista_nits.Add(new { Nit = un_nit, IvaEmitido = iva_dado, IvaRecibido = iva_cobrado });
            }

            return Ok(lista_nits);
        }

        [HttpGet("totales-fecha")]
        public IActionResult TotalesPorFecha(string inicio, string fin, bool ver_con_iva)
        {
            var facturas = control_archivos.TraerFacturasGuardadas();
            DateTime fecha1 = DateTime.Parse(inicio);
            DateTime fecha2 = DateTime.Parse(fin);

            var buenas = facturas.Where(f => !f.TieneErrores && f.FechaHoraReal.Date >= fecha1.Date && f.FechaHoraReal.Date <= fecha2.Date);

            var agrupado = buenas.GroupBy(f => f.FechaHoraReal.Date).Select(g => new
            {
                Fecha = g.Key.ToString("dd/MM/yyyy"),
                Valor = ver_con_iva ? g.Sum(x => x.TotalConIva) : g.Sum(x => x.MontoSinIva)
            }).ToList();

            return Ok(agrupado);
        }
    }
}