using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InfoLibro.Utilidades;

/// <summary>
/// Identidad visual de InfoLibro (colores y fuentes del Figma).
/// Para cambiar la fuente del logo/títulos, edita FuentesTitulo o FuentesTexto.
/// </summary>
public static class Tema
{
    // ---- Colores ----
    public static readonly Color Naranja = Color.FromArgb(248, 169, 107);
    public static readonly Color NaranjaOscuro = Color.FromArgb(230, 140, 70);
    public static readonly Color NaranjaClaro = Color.FromArgb(253, 235, 219);
    public static readonly Color Cafe = Color.FromArgb(52, 32, 20);
    public static readonly Color ColorTexto = Color.FromArgb(40, 40, 40);
    public static readonly Color ColorTextoSuave = Color.FromArgb(120, 120, 120);
    public static readonly Color Verde = Color.FromArgb(0, 158, 96);
    public static readonly Color Rojo = Color.FromArgb(229, 57, 53);
    public static readonly Color FondoSuave = Color.FromArgb(253, 247, 242);

    // ---- Fuentes (se usa la primera que exista; si no, el respaldo) ----
    public static readonly string[] FuentesTitulo =
        { "UnifrakturCook", "Pirata One", "Jacquard 12", "MedievalSharp", "Uncial Antiqua", "Blackadder ITC", "Old English Text MT" };
    public static readonly string[] FuentesTexto = { "Comfortaa" };
    // Factor de tamaño del texto. 1.0 = tamaño del diseño. Si lo quieres más grande, prueba 1.1 o 1.15 (no más).
    public static float EscalaTexto = 1.0f;

    const string RespaldoTitulo = "Georgia";
    const string RespaldoTexto = "Segoe UI";

    static readonly PrivateFontCollection privadas = new PrivateFontCollection();
    static FontFamily[] instaladas;

    /// <summary>Carga los .ttf/.otf que se pongan en Recursos\Fuentes.</summary>
    public static void Inicializar()
    {
        try
        {
            string carpeta = Path.Combine(AppContext.BaseDirectory, "Recursos", "Fuentes");
            if (!Directory.Exists(carpeta)) return;
            foreach (string archivo in Directory.GetFiles(carpeta))
            {
                string ext = Path.GetExtension(archivo).ToLowerInvariant();
                if (ext != ".ttf" && ext != ".otf") continue;
                try { privadas.AddFontFile(archivo); } catch { }
            }
        }
        catch { }
    }

    static FontFamily Buscar(string[] candidatas, string respaldo)
    {
        instaladas ??= new InstalledFontCollection().Families;
        foreach (string nombre in candidatas)
        {
            FontFamily f = privadas.Families.FirstOrDefault(x => x.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                        ?? instaladas.FirstOrDefault(x => x.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (f != null) return f;
        }
        try { return new FontFamily(respaldo); }
        catch { return FontFamily.GenericSansSerif; }
    }

    static Font Crear(FontFamily familia, float tam, FontStyle estilo)
    {
        if (!familia.IsStyleAvailable(estilo))
        {
            foreach (FontStyle s in new[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic })
            {
                if (familia.IsStyleAvailable(s)) { estilo = s; break; }
            }
        }
        // Se usa PÍXELES (no puntos) para que el texto no crezca con el escalado de Windows (125%/150%)
        // y no se corte dentro de las cajas, cuyo tamaño está en píxeles.
        return new Font(familia, tam * EscalaTexto * 96f / 72f, estilo, GraphicsUnit.Pixel);
    }

    /// <summary>Fuente gótica del logo y títulos destacados.</summary>
    public static Font FuenteTitulo(float tam)
    {
        FontFamily f = Buscar(FuentesTitulo, RespaldoTitulo);
        FontStyle estilo = f.Name == RespaldoTitulo ? FontStyle.Bold : FontStyle.Regular;
        return Crear(f, tam, estilo);
    }

    /// <summary>Fuente redondeada del texto general (Comfortaa en el Figma).</summary>
    public static Font FuenteTexto(float tam, FontStyle estilo = FontStyle.Regular)
    {
        return Crear(Buscar(FuentesTexto, RespaldoTexto), tam, estilo);
    }

    /// <summary>Fuente para TextBox y tablas (siempre una fuente instalada en Windows).</summary>
    public static Font FuenteCampo(float tam, FontStyle estilo = FontStyle.Regular)
    {
        return new Font("Segoe UI", tam * EscalaTexto * 96f / 72f, estilo, GraphicsUnit.Pixel);
    }

    /// <summary>Crea un Label transparente listo para usar sobre fondos dibujados.</summary>
    public static Label Etiqueta(string texto, Font fuente, Color color)
    {
        return new Label
        {
            Text = texto,
            Font = fuente,
            ForeColor = color,
            BackColor = Color.Transparent,
            AutoSize = false,
            UseCompatibleTextRendering = true   // necesario para fuentes cargadas desde archivo
        };
    }

    public static GraphicsPath RectRedondeado(Rectangle r, int radio)
    {
        int d = Math.Max(1, Math.Min(radio * 2, Math.Min(r.Width, r.Height)));
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>Dibuja una imagen llenando todo el rectángulo (recorta lo que sobra).</summary>
    public static void DibujarCubierto(Graphics g, Image imagen, Rectangle destino, float anclaVertical = 0.5f)
    {
        if (imagen == null)
        {
            using var b = new SolidBrush(Cafe);
            g.FillRectangle(b, destino);
            return;
        }

        float escala = Math.Max((float)destino.Width / imagen.Width, (float)destino.Height / imagen.Height);
        float srcW = destino.Width / escala;
        float srcH = destino.Height / escala;
        float srcX = (imagen.Width - srcW) / 2f;
        float srcY = (imagen.Height - srcH) * anclaVertical;

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawImage(imagen, destino, srcX, srcY, srcW, srcH, GraphicsUnit.Pixel);
    }

    /// <summary>Estilo común de las tablas (DataGridView).</summary>
    public static void EstiloTabla(DataGridView g)
    {
        g.BackgroundColor = Color.White;
        g.BorderStyle = BorderStyle.None;
        g.EnableHeadersVisualStyles = false;
        g.RowHeadersVisible = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.ReadOnly = true;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.GridColor = NaranjaClaro;

        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersHeight = 40;
        g.ColumnHeadersDefaultCellStyle.BackColor = Naranja;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Naranja;
        g.ColumnHeadersDefaultCellStyle.Font = FuenteCampo(10.5f, FontStyle.Bold);

        g.RowTemplate.Height = 36;
        g.DefaultCellStyle.Font = FuenteCampo(10.5f);
        g.DefaultCellStyle.ForeColor = ColorTexto;
        g.DefaultCellStyle.SelectionBackColor = NaranjaClaro;
        g.DefaultCellStyle.SelectionForeColor = ColorTexto;
        g.AlternatingRowsDefaultCellStyle.BackColor = FondoSuave;
    }
}
