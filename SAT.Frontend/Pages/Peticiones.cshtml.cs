using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SAT.Frontend.Pages
{
    public class PeticionesModel : PageModel
    {
        public string UrlApiReportes { get; set; } = "https://localhost:7239/api/reportes";

        public void OnGet()
        {
            
        }
    }
}