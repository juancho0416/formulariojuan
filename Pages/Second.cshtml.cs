using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;



namespace form.Pages;

public class SecondModel : PageModel
{
    private readonly HttpClient _httpClient;

    public SecondModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [BindProperty]
    public FormInput Input { get; set; } = new FormInput();

    [BindProperty]
    public string CodigoPostal { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;

    public void OnGet()
    {
        // Si vienes de otra página y traes valores en TempData, opcionalmente restáuralos
        if (TempData.ContainsKey("Estado")) Estado = TempData.Peek("Estado")?.ToString() ?? string.Empty;
        if (TempData.ContainsKey("Municipio")) Municipio = TempData.Peek("Municipio")?.ToString() ?? string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Guardar datos en TempData
        TempData["Nombre"] = Input.Nombre;
        TempData["RFC"] = Input.RFC;
        TempData["CURP"] = Input.CURP;
        TempData["Telefono"] = Input.Telefono;
        TempData["Folio"] = Input.Folio;

        // Validar CP con API SEPOMEX (mismo comportamiento si un usuario presiona Enviar)
        if (!string.IsNullOrEmpty(CodigoPostal))
        {
            var url = $"https://api.zippopotam.us/mx/{CodigoPostal}";
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                using var json = JsonDocument.Parse(response);
                var root = json.RootElement;

                Estado = root.GetProperty("places")[0].GetProperty("state").GetString() ?? string.Empty;
                Municipio = root.GetProperty("places")[0].GetProperty("place name").GetString() ?? string.Empty;
            }
            catch
            {
                Error = "No se pudo obtener datos del Código Postal.";
                return Page();
            }
        }

        // Guardar CP / Estado / Municipio en TempData antes de redirigir
        TempData["CodigoPostal"] = CodigoPostal ?? string.Empty;
        TempData["Estado"] = Estado ?? string.Empty;
        TempData["Municipio"] = Municipio ?? string.Empty;

        // Redirigir a la siguiente página
        return RedirectToPage("TwoFive");
    }

    // Nuevo handler (GET) que responde con JSON para llenar Estado/Municipio
    public async Task<JsonResult> OnGetFetchPostalAsync(string codigoPostal)
    {
        if (string.IsNullOrWhiteSpace(codigoPostal))
        {
            return new JsonResult(new { ok = false, error = "Código postal vacío" });
        }

        var url = $"https://api.zippopotam.us/mx/{codigoPostal}";
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            using var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            var state = root.GetProperty("places")[0].GetProperty("state").GetString() ?? string.Empty;
            var places = root.GetProperty("places").EnumerateArray()
                .Select(p => p.GetProperty("place name").GetString() ?? string.Empty)
                .Distinct()
                .ToArray();

            return new JsonResult(new { ok = true, state, municipios = places });
        }
        catch
        {
            return new JsonResult(new { ok = false, error = "No se encontró el código postal o fallo al consultar el servicio." });
        }
    }

    public class FormInput
    {
        [Required(ErrorMessage = "Nombre completo requerido")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'´`-]{2,}(?:\s+[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'´`-]+)*$",
            ErrorMessage = "El nombre contiene caracteres no permitidos o es demasiado corto")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "RFC requerido")]
        [RegularExpression(@"^[A-Z]{3,4}\d{6}[A-Z0-9]{3}$",
            ErrorMessage = "El RFC no tiene el formato válido")]
        [Display(Name = "RFC")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "CURP requerido")]
        [RegularExpression(@"^[A-Z]{4}\d{6}[HM][A-Z]{5}[A-Z0-9]\d$",
            ErrorMessage = "El formato del CURP no es válido")]
        [Display(Name = "CURP")]
        public string CURP { get; set; } = string.Empty;

        [Required(ErrorMessage = "Folio requerido")]
        [RegularExpression(@"^[1-9]{8}$",
            ErrorMessage = "El folio no tiene el formato válido")]
        [Display(Name = "Folio")]
        public string Folio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teléfono requerido")]
        [RegularExpression(@"^(?=.*\d{7,15})[0-9+\-\s()]+$",
            ErrorMessage = "El número de teléfono contiene caracteres no permitidos o es demasiado corto")]
        [Display(Name = "Teléfono celular")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Es necesario ingresar el nombre de la calle")]
        [Display(Name = "Calle")]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Es necesario ingresar el numero de la calle")]
        [Display(Name = "Numero")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Numero2")]
        public string Numero2 { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Es necesario ingresar denominacion o razon social")]
        [Display(Name = "RazonSocial")]
        public string RazonSocial { get; set; } = string.Empty;

    }
}
