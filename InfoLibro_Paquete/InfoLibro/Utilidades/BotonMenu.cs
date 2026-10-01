using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InfoLibro.Utilidades;

/// <summary>Botón del menú lateral: icono blanco + texto, resaltado cuando está seleccionado.</summary>
public class BotonMenu : Control
{
    bool sobre;
    bool seleccionado;

    public Image Icono { get; set; }

    public bool Seleccionado
    {
        get => seleccionado;
        set { seleccionado = value; Invalidate(); }
    }

    public BotonMenu()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
        BackColor = Color.Transparent;
        Size = new Size(240, 54);
        Cursor = Cursors.Hand;
        Font = Tema.FuenteTexto(12f, FontStyle.Bold);
    }

    protected override bool IsInputKey(Keys keyData) => keyData == Keys.Enter || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            OnClick(EventArgs.Empty);
        }
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); sobre = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); sobre = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        var rect = new Rectangle(14, 4, Width - 28, Height - 8);

        if (seleccionado || sobre)
        {
            int alfa = seleccionado ? 90 : 45;
            using var path = Tema.RectRedondeado(rect, 14);
            using var brocha = new SolidBrush(Color.FromArgb(alfa, 255, 255, 255));
            g.FillPath(brocha, path);
        }

        if (Icono != null)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(Icono, new Rectangle(rect.X + 16, (Height - 28) / 2, 28, 28));
        }

        using var formato = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };
        g.DrawString(Text, Font, Brushes.White,
            new RectangleF(rect.X + 58, 0, Math.Max(1, rect.Width - 62), Height), formato);
    }
}
