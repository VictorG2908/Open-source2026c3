using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Modelos;

namespace InfoLibro.Seguridad;

public class PermisoDenegadoException : Exception
{
    public PermisoDenegadoException(string mensaje) : base(mensaje) { }
}

/// <summary>
/// Control de acceso. Ocultar un botón NO basta: cada operación de escritura
/// debe llamar a Exigir() antes de tocar la base de datos.
/// </summary>
public static class Permisos
{
    /// <summary>¿El rol de la sesión actual permite esta acción?</summary>
    public static bool Tiene(Accion accion)
    {
        return Sesion.RolActual != null && Sesion.RolActual.Permite(accion);
    }

    /// <summary>Lanza PermisoDenegadoException si el rol no tiene permiso.</summary>
    public static void Exigir(Accion accion, string descripcion)
    {
        if (Tiene(accion)) return;
        string rol = Sesion.RolActual?.NombreRol ?? "sin rol";
        throw new PermisoDenegadoException(
            $"No tienes permisos suficientes para {descripcion}.\n\nTu rol ({rol}) no incluye la acción \"{accion}\".");
    }

    /// <summary>Versión para formularios: muestra el MessageBox y devuelve true/false.</summary>
    public static bool Verificar(Accion accion, string descripcion)
    {
        try
        {
            Exigir(accion, descripcion);
            return true;
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }
}
