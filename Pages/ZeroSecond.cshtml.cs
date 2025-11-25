using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace form.Pages
{
    public class ZeroSecond : PageModel
    {
        [BindProperty]
        public FormInput Input { get; set; } = new FormInput();

        public void OnGet() { }

        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }


            // Guardar la fecha en TempData
            TempData["Fecha"] = Input.Fecha.ToString();

            // Redirigir a otra página
            return RedirectToPage("Second");
        }

        public class FormInput
        {
            [Required(ErrorMessage = "Es necesario seleccionar una fecha")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha")]
            public DateOnly Fecha { get; set; }

        }
    }
}
