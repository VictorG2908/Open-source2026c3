using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Modelos;
using InfoLibro.Datos;

namespace InfoLibro.Seguridad;

/// <summary>Guarda quién inició sesión mientras la aplicación está abierta.</summary>
public static class Sesion
{
    public static Usuario UsuarioActual { get; private set; }
    public static Rol RolActual => UsuarioActual?.Rol;
    public static bool HayLogin => UsuarioActual != null;

    public static void Iniciar(Usuario usuario)
        {
            // Asegurar que UsuarioActual tenga el Rol cargado para evitar NullReference en la UI
            if (usuario != null && usuario.Rol == null && usuario.IdRol != 0)
            {
                try
                {
                    usuario.Rol = RolDatos.ObtenerPorId(usuario.IdRol);
                }
                catch
                {
                    // Ignorar errores al recuperar el rol; la UI usará comprobaciones nulas
                }
            }
            UsuarioActual = usuario;
        }
    public static void Cerrar() => UsuarioActual = null;
}
