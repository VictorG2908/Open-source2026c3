namespace InfoLibro.Utilidades;

public static class Validador
{
    public static bool Vacio(string texto) => string.IsNullOrWhiteSpace(texto);

    public static bool UsuarioValido(string texto)
    {
        return !Vacio(texto) && texto.Length >= 4 && texto.Length <= 20
            && texto.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
    }

    public static bool ClaveValida(string texto) => !Vacio(texto) && texto.Length >= 6;

    public static bool CorreoValido(string texto)
    {
        if (Vacio(texto)) return false;
        try
        {
            var direccion = new System.Net.Mail.MailAddress(texto.Trim());
            return direccion.Address == texto.Trim();
        }
        catch
        {
            return false;
        }
    }
}
