using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace form.Pages
{
    public class TwoNineModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public TwoNineModel(IWebHostEnvironment env) => _env = env;

        [BindProperty]
        public IFormFile? Archivo { get; set; }

        public string? TipoFormulario { get; set; }

        public void OnGet(string? tipo)
        {
            TipoFormulario = tipo;
            // usar TipoFormulario para adaptar la UI o cargar plantilla
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (Archivo == null || Archivo.Length == 0)
            {
                TempData["UploadMessage"] = "Seleccione un archivo.";
                TempData["ShowUploadModal"] = true;
                return Page();
            }

            var ext = Path.GetExtension(Archivo.FileName)?.ToLowerInvariant();
            var permitidos = new[] { ".json", ".xml", ".pdf" };
            if (string.IsNullOrEmpty(ext) || !permitidos.Contains(ext))
            {
                TempData["UploadMessage"] = "Formato no permitido. Use .json, .xml o .pdf";
                TempData["ShowUploadModal"] = true;
                return Page();
            }

            const long maxBytes = 10 * 1024 * 1024; // 10 MB
            if (Archivo.Length > maxBytes)
            {
                TempData["UploadMessage"] = "Archivo demasiado grande. Máximo 10 MB.";
                TempData["ShowUploadModal"] = true;
                return Page();
            }

            var uploads = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            Directory.CreateDirectory(uploads);
            var safeFileName = Path.GetFileName(Archivo.FileName);
            var filePath = Path.Combine(uploads, safeFileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await Archivo.CopyToAsync(stream);
            }

            TempData["UploadMessage"] = "Archivo subido correctamente.";
            TempData["UploadedFile"] = safeFileName; // guarda el nombre
            TempData["ShowUploadModal"] = true;

            //se redirige al third al dar en continuar con el modal
            return Page();
        }
    }
}



























