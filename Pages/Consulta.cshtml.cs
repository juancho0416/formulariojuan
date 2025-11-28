using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace form.Pages
{
    public class ConsultaModel : PageModel
    {
        private readonly EmailService _email;

        public ConsultaModel(EmailService email)
        {
            _email = email;
        }

        [BindProperty]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string CorreoBusqueda { get; set; } = string.Empty;

        public List<Dictionary<string, string>> Formularios { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;

        public void OnGet() { }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(CorreoBusqueda))
            {
                Mensaje = "Ingrese un correo para buscar.";
                return;
            }

            Formularios = ObtenerFormularios(CorreoBusqueda);
            Mensaje = Formularios.Count > 0 ? "Datos encontrados." : "No se encontró información relacionada a ese correo.";
        }

        // Handler para enviar correo con todos los formularios
        public async Task<IActionResult> OnPostEnviarCorreoAsync()
        {
            if (string.IsNullOrWhiteSpace(CorreoBusqueda))
            {
                Mensaje = "Ingrese un correo para enviar.";
                return Page();
            }

            var formularios = ObtenerFormularios(CorreoBusqueda);

            if (formularios.Count == 0)
            {
                Mensaje = "No se encontró información relacionada a ese correo.";
                return Page();
            }

            var builder = new StringBuilder();
            builder.Append("<h2>Todos tus formularios</h2>");

            foreach (var form in formularios)
            {
                builder.Append("<hr/>");
                foreach (var kv in form)
                {
                    builder.Append($"<p><strong>{kv.Key}:</strong> {kv.Value}</p>");
                }
            }

            await _email.EnviarCorreoGenerico(
                CorreoBusqueda,
                "Consulta de formularios",
                builder.ToString()
            );

            Mensaje = "Correo enviado correctamente.";
            return Page();
        }

        //  Método reutilizable para obtener formularios
        private List<Dictionary<string, string>> ObtenerFormularios(string correo)
        {
            var lista = new List<Dictionary<string, string>>();

            using var connection = new SqliteConnection("Data Source=usuarios.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT *
                FROM Formularios
                WHERE Correo = $correo
                ORDER BY Id DESC;
            ";
            command.Parameters.AddWithValue("$correo", correo.Trim().ToLower());

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var datos = new Dictionary<string, string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columna = reader.GetName(i);
                    string valor = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                    datos[columna] = valor;
                }
                lista.Add(datos);
            }

            return lista;
        }
    }
}
