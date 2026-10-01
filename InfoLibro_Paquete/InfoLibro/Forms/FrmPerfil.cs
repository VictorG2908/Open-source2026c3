using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Perfil del usuario actual (solo lectura por ahora). Inspirado en la pantalla Perfil del Figma.</summary>
public class FrmPerfil : Form
{
    readonly Image fondo = Recurso.Imagen("fondo.png");

    public FrmPerfil()
    {
        BackColor = Color.White;
        Usuario u = Sesion.UsuarioActual;

        var banner = new PanelDoble { Dock = DockStyle.Top, Height = 300 };
        banner.Paint += (s, e) => PintarBanner(e.Graphics, banner.ClientRectangle, u);

        var info = new PanelDoble { Dock = DockStyle.Fill, BackColor = Color.White };
        string[,] datos =
        {
            { "Nombre completo", u.NombreCompleto },
            { "Correo", u.Correo },
            { "Rol", Sesion.RolActual.NombreRol },
            { "Estado", u.Activo ? "Activo" : "Inactivo" },
        };
        int y = 28;
        for (int i = 0; i < datos.GetLength(0); i++)
        {
            Label etiqueta = Tema.Etiqueta(datos[i, 0], Tema.FuenteTexto(10f, FontStyle.Bold), Tema.ColorTextoSuave);
            etiqueta.SetBounds(60, y, 200, 28);
            Label valor = Tema.Etiqueta(datos[i, 1] ?? "", Tema.FuenteTexto(12f), Tema.ColorTexto);
            valor.SetBounds(260, y, 600, 28);
            info.Controls.Add(etiqueta);
            info.Controls.Add(valor);
            y += 40;
        }

        Controls.Add(info);     // el que llena va primero
        Controls.Add(banner);
    }

    void PintarBanner(Graphics g, Rectangle r, Usuario u)
    {
        if (r.Width < 2 || r.Height < 2) return;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        Tema.DibujarCubierto(g, fondo, r, 0.35f);
        using (var velo = new SolidBrush(Color.FromArgb(150, 20, 10, 5)))
            g.FillRectangle(velo, r);

        int cx = r.Width / 2;
        var avatar = new Rectangle(cx - 50, 28, 100, 100);
        using (var b = new SolidBrush(Tema.Naranja)) g.FillEllipse(b, avatar);
        using (var p = new Pen(Color.White, 3f)) g.DrawEllipse(p, avatar);

        using var centro = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        string nombre = u.NombreCompleto ?? u.NombreUsuario ?? "";
        string inicial = nombre.Length > 0 ? nombre.Substring(0, 1).ToUpper() : "?";

        using (var f = Tema.FuenteTexto(40f, FontStyle.Bold))
            g.DrawString(inicial, f, Brushes.White, avatar, centro);
        using (var f = Tema.FuenteTexto(22f, FontStyle.Bold))
            g.DrawString(nombre, f, Brushes.White, new RectangleF(0, 140, r.Width, 40), centro);
        using (var f = Tema.FuenteTexto(12f))
            g.DrawString("@" + u.NombreUsuario, f, Brushes.White, new RectangleF(0, 180, r.Width, 26), centro);
        using (var f = Tema.FuenteTitulo(28f))
            g.DrawString("Miembro desde " + u.FechaRegistro.ToString("dd/MM/yy"), f, Brushes.White,
                new RectangleF(0, 222, r.Width, 52), centro);
    }
}
