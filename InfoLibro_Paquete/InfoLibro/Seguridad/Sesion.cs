using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Modelos;

namespace InfoLibro.Seguridad;

/// <summary>Guarda quién inició sesión mientras la aplicación está abierta.</summary>
public static class Sesion
{
    public static Usuario UsuarioActual { get; private set; }
    public static Rol RolActual => UsuarioActual?.Rol;
    public static bool HayLogin => UsuarioActual != null;

    public static void Iniciar(Usuario usuario) => UsuarioActual = usuario;
    public static void Cerrar() => UsuarioActual = null;
}
