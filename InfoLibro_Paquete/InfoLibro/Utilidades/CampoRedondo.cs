using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace InfoLibro.Utilidades;

/// <summary>Caja de texto con forma de píldora (como en el login del Figma).</summary>
public class CampoRedondo : Control
{
    readonly TextBox caja = new TextBox();
    Color relleno = Color.White;
    Color borde = Color.Transparent;
    bool enfocado;

    public Color ColorRelleno
    {
        get => relleno;
        set { relleno = value; caja.BackColor = value; Invalidate(); }
    }

    public Color ColorBorde
    {
        get => borde;
        set { borde = value; Invalidate(); }
    }

    public string Valor
    {
        get => caja.Text;
        set => caja.Text = value;
    }

    public string Placeholder
    {
        get => caja.PlaceholderText;
        set => caja.PlaceholderText = value;
    }

    /// <summary>true = oculta los caracteres (contraseña).</summary>
    public bool EsClave
    {
        get => caja.UseSystemPasswordChar;
        set => caja.UseSystemPasswordChar = value;
    }

    public TextBox Caja => caja;

    public CampoRedondo()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        TabStop = false;

        caja.BorderStyle = BorderStyle.None;
        caja.BackColor = relleno;
        caja.ForeColor = Tema.ColorTexto;
        caja.Font = Tema.FuenteCampo(12f);
        caja.Enter += (s, e) => { enfocado = true; Invalidate(); };
        caja.Leave += (s, e) => { enfocado = false; Invalidate(); };
        Controls.Add(caja);

        Size = new Size(300, 46);   // al final: así el TextBox ya tiene su fuente y su altura
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        int margen = Math.Max(10, Height / 2 - 4);
        caja.Width = Math.Max(10, Width - margen * 2);
        caja.Left = margen;
        caja.Top = Math.Max(0, (Height - caja.Height) / 2);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        caja.Focus();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = Tema.RectRedondeado(rect, Height / 2);
        using var fondo = new SolidBrush(relleno);
        g.FillPath(fondo, path);

        Color colorBorde = enfocado ? Tema.NaranjaOscuro : borde;
        if (colorBorde.A > 0)
        {
            using var lapiz = new Pen(colorBorde, 2f);
            g.DrawPath(lapiz, path);
        }
    }
}
