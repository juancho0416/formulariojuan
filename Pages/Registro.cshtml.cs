using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace form.Pages;

public class RegistroModel : PageModel
{
    private readonly IConfiguration _config;
    private readonly EmailService _email;

    public RegistroModel(IConfiguration config, EmailService email)
    {
        _config = config;
        _email = email;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        string correoNormalizado = Input.Correo.Trim().ToLower();

        using var connection = new SqliteConnection("Data Source=usuarios.db");
        connection.Open();

        //  Verificar si el correo ya existe
        var check = connection.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM Usuarios WHERE Correo = $correo";
        check.Parameters.AddWithValue("$correo", correoNormalizado);

        long count = (long)check.ExecuteScalar();

        if (count > 0)
        {
            ErrorMessage = "El correo ya está registrado.";
            return Page();
        }

        // Hash de contraseña
        string hashedPassword = HashPassword(Input.Contraseña);

        // Generar token de confirmación
        string token = Guid.NewGuid().ToString();

        // Insertar usuario NO confirmado
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Usuarios (Nombre, Apellido, Correo, Contrasena, TokenConfirmacion, Confirmado)
            VALUES ($nombre, $apellido, $correo, $contrasena, $token, 0);
        ";

        command.Parameters.AddWithValue("$nombre", Input.Nombre);
        command.Parameters.AddWithValue("$apellido", Input.Apellido);
        command.Parameters.AddWithValue("$correo", correoNormalizado);
        command.Parameters.AddWithValue("$contrasena", hashedPassword);
        command.Parameters.AddWithValue("$token", token);

        command.ExecuteNonQuery();

        //  Enviar correo de confirmación con Resend
        await _email.EnviarCorreoConfirmacion(correoNormalizado, token);
        return RedirectToPage("/ConfirmacionEnviada");

    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public class InputModel
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}
