using System.Drawing;
using System.Windows.Forms;
namespace InfoLibro.Utilidades;

/// <summary>Carga imágenes desde la carpeta Recursos (junto al .exe). Devuelve null si no existe.</summary>
public static class Recurso
{
    static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

    public static Image Imagen(string nombreArchivo)
    {
        if (cache.TryGetValue(nombreArchivo, out Image existente)) return existente;

        Image resultado = null;
        try
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "Recursos", nombreArchivo);
            if (File.Exists(ruta))
            {
                using var flujo = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                using var temporal = Image.FromStream(flujo);
                resultado = new Bitmap(temporal);   // copia, para no dejar el archivo bloqueado
            }
        }
        catch
        {
            resultado = null;
        }

        cache[nombreArchivo] = resultado;
        return resultado;
    }
}
