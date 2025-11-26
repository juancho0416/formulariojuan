using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace form.Pages
{
    public class ConsultaModel : PageModel
    {
        [BindProperty]
        public string CorreoBusqueda { get; set; } = string.Empty;

        public Dictionary<string, string> DatosFormulario { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;

        public void OnGet() { }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(CorreoBusqueda))
            {
                Mensaje = "Ingrese un correo para buscar.";
                return;
            }

            using var connection = new SqliteConnection("Data Source=usuarios.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT *
                FROM Formularios
                WHERE Correo = $correo
                ORDER BY Id DESC
                LIMIT 1;
            ";

            command.Parameters.AddWithValue("$correo", CorreoBusqueda.Trim().ToLower());

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                Mensaje = "No se encontró información relacionada a ese correo.";
                return;
            }

            // ✅ Convertir todos los campos en un diccionario
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columna = reader.GetName(i);
                string valor = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                DatosFormulario[columna] = valor;
            }

            Mensaje = "Datos encontrados.";
        }
    }
}
