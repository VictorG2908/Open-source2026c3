using System.Security.Cryptography;
using System.Text;

namespace InfoLibro.Seguridad;

/// <summary>
/// Las contraseñas NUNCA se guardan en texto plano: se guarda SHA-256(salt + clave).
/// El salt es un texto aleatorio distinto para cada usuario.
/// </summary>
public static class Hash
{
    public static string GenerarSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    }

    public static string Calcular(string clave, string salt)
    {
        byte[] datos = Encoding.UTF8.GetBytes(salt + clave);
        return Convert.ToHexString(SHA256.HashData(datos));
    }
}
