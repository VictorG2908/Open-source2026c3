using Microsoft.Data.SqlClient;

namespace InfoLibro.Datos;

/// <summary>Conexión a SQL Server. Es el ÚNICO lugar donde se configura el servidor.</summary>
public static class Conexion
{
    // Intento automático de varias cadenas de conexión comunes.
    // La aplicación probará estas opciones en orden y seleccionará la primera que funcione.
    // Si necesitas una cadena personalizada, modifica manualmente el valor en tiempo de diseño
    // o establece Conexion.CadenaConexion antes de usarla.
    public static string CadenaConexion { get; set; }

    static Conexion()
    {
        var candidates = new[]
        {
            // LocalDB (Visual Studio / LocalDB)
            "Server=(localdb)\\MSSQLLocalDB;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;",
            // SQL Server Express
            "Server=.\\SQLEXPRESS;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;",
            // Nombre de instancia por defecto / servicio SQL Server
            "Server=localhost;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;",
            // Fallback: localhost con autenticación SQL (intenta sa sin contraseña no es recomendable)
            // Si usas usuario/contraseña de SQL Server, reemplaza User Id/Password aquí.
            "Server=localhost;Database=InfoLibro;User Id=sa;Password=;TrustServerCertificate=True;"
        };

        foreach (var c in candidates)
        {
            CadenaConexion = c;
            try
            {
                using var cn = Crear();
                cn.Open();
                // conexión válida
                return;
            }
            catch
            {
                // ignorar y probar la siguiente
            }
        }

        // Si ninguna funcionó, dejar la primera por defecto y la aplicación mostrará el error al intentar abrir.
        CadenaConexion = candidates[0];
    }

    public static SqlConnection Crear() => new SqlConnection(CadenaConexion);

    /// <summary>Intenta abrir la conexión. Devuelve el mensaje de error si falla.</summary>
    public static bool Probar(out string error)
    {
        try
        {
            using var cn = Crear();
            cn.Open();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
