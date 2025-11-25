using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace form.Pages;

public class ThirdModel : PageModel
{
    private readonly IWebHostEnvironment _env;
    public ThirdModel(IWebHostEnvironment env) => _env = env;

    // propiedades que usa la vista
    //Seleccion de fecha
    public string? Fecha { get; set; }

    //Primera pagina formulario (Second)
    public string? Nombre { get; set; }
    public string? RFC { get; set; }
    public string? CURP { get; set; }
    public string? Folio { get; set; }
    public string? Telefono { get; set; }
    //segunda parte formulario (TwoFive)
    public string? Area { get; set; }
    public string? Empresa { get; set; }
    public string? ISO { get; set; }
    public string? NOM { get; set; }
    public string? Contrato { get; set; }
    public string? Requerimiento { get; set; }
    public string? Permiso { get; set; }
    public string? Peticion { get; set; }
    //Tercera parte formuylario(TwoNineFive)

    public string? Regulacion { get; set; }
    public string? Ley { get; set; }
    public string? Articulo { get; set; }
    public string? Parrafo { get; set; }


    [TempData] public string CodigoPostal { get; set; } = string.Empty;
    [TempData] public string Estado { get; set; } = string.Empty;
    [TempData] public string Municipio { get; set; } = string.Empty;



    //parte de subir archivos validos
    public string? Archivo { get; set; }
    public bool FileExists { get; set; }
    public string? UploadMessage { get; set; }

    // único OnGet: acepta parámetros opcionales (vienen por query string) o usa TempData
    public void OnGet(string? uploadedFile, string? message)
    {


        ///temp data guarda la informacion de los formualrios anteriores en esta pagina 
        /// con tempdata 

        /// /// Primera parte del formulario
        Nombre = TempData["Nombre"]?.ToString();
        RFC = TempData["RFC"]?.ToString();
        CURP = TempData["CURP"]?.ToString();
        Telefono = TempData["Telefono"]?.ToString();
        Folio = TempData["Folio"]?.ToString();

        ///Parte dos del formulario desplegables
        Area = TempData["Area"]?.ToString();
        Empresa = TempData["Empresa"]?.ToString();
        ISO = TempData["ISO"]?.ToString();
        NOM = TempData["NOM"]?.ToString();
        Contrato = TempData["Contrato"]?.ToString();
        Requerimiento = TempData["Requerimiento"]?.ToString();
        Permiso = TempData["Permiso"]?.ToString();
        Peticion = TempData["Peticion"]?.ToString();
        //tercera parte formulario 
        Regulacion = TempData["Regulacion"]?.ToString();
        Ley = TempData["Ley"]?.ToString();
        Articulo = TempData["Articulo"]?.ToString();
        Parrafo = TempData["Parrafo"]?.ToString();

        Fecha = TempData["Fecha"]?.ToString();








        // Prioriza el querystring, si no existe usa TempData (por si viniste desde TwoNine)
        Archivo = !string.IsNullOrWhiteSpace(uploadedFile) ? uploadedFile : TempData["UploadedFile"] as string;
        UploadMessage = !string.IsNullOrWhiteSpace(message) ? message : TempData["UploadMessage"] as string;

        if (!string.IsNullOrWhiteSpace(Archivo))
        {
            var safeName = Path.GetFileName(Archivo);
            var uploads = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            var fullPath = Path.Combine(uploads, safeName);

            FileExists = System.IO.File.Exists(fullPath);
            Archivo = safeName;
            if (!FileExists)
            {
                UploadMessage = "No se encontró el archivo subido.";
            }
            else
            {
                UploadMessage ??= "Archivo encontrado y listo para validar.";
            }
        }
        else
        {
            FileExists = false;
            UploadMessage ??= "No se recibió nombre de archivo.";
        }

        // deja valores en TempData si otras vistas los necesitan
        TempData["UploadedFile"] = Archivo;
        TempData["UploadMessage"] = UploadMessage;
    }

    // opcional: manejar POST de confirmación
    public IActionResult OnPostConfirm()
    {
        // ...acción de confirmación...
        return RedirectToPage("/Index");
    }










    public IActionResult OnPost()
    {
        //fecha

        //primera parte del formulario
        Nombre = TempData["Nombre"]?.ToString();
        RFC = TempData["RFC"]?.ToString();
        CURP = TempData["CURP"]?.ToString();
        Telefono = TempData["Telefono"]?.ToString();

        Folio = TempData["Folio"]?.ToString();
        //segunda parte del formulario
        Area = TempData["Area"]?.ToString();
        Empresa = TempData["Empresa"]?.ToString();
        ISO = TempData["ISO"]?.ToString();
        NOM = TempData["NOM"]?.ToString();
        Contrato = TempData["Contrato"]?.ToString();
        Requerimiento = TempData["Requerimiento"]?.ToString();
        Permiso = TempData["Permiso"]?.ToString();
        Peticion = TempData["Peticion"]?.ToString();
        //tercera parte del formulario
        Regulacion = TempData["Regulacion"]?.ToString();
        Ley = TempData["Ley"]?.ToString();
        Articulo = TempData["Articulo"]?.ToString();
        Parrafo = TempData["Parrafo"]?.ToString();


        Fecha = TempData["Fecha"]?.ToString();



        return RedirectToPage("Fourth");

    }
}