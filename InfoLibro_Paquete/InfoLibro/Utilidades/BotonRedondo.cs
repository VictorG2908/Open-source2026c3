using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InfoLibro.Utilidades;

/// <summary>Botón con forma de píldora. Con SoloLectura = true se usa como etiqueta (ej. el rol).</summary>
public class BotonRedondo : Control
{
    bool sobre;

    public Color ColorNormal { get; set; } = Tema.Naranja;
    public Color ColorHover { get; set; } = Tema.NaranjaOscuro;
    public Color ColorLetra { get; set; } = Color.White;
    public Color ColorBorde { get; set; } = Color.Transparent;
    public bool SoloLectura { get; set; }

    public BotonRedondo()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
        BackColor = Color.Transparent;
        Size = new Size(200, 46);
        Font = Tema.FuenteTexto(12f, FontStyle.Bold);
    }

    protected override bool IsInputKey(Keys keyData) => keyData == Keys.Enter || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!SoloLectura && (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter))
        {
            e.Handled = true;
            OnClick(EventArgs.Empty);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        sobre = true;
        Cursor = SoloLectura ? Cursors.Default : Cursors.Hand;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        sobre = false;
        Invalidate();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        Color relleno = !Enabled ? Color.FromArgb(205, 205, 205)
                      : (sobre && !SoloLectura ? ColorHover : ColorNormal);

        using (var path = Tema.RectRedondeado(rect, Height / 2))
        using (var brocha = new SolidBrush(relleno))
        {
            g.FillPath(brocha, path);
            if (ColorBorde.A > 0)
            {
                using var lapiz = new Pen(ColorBorde, 2f);
                g.DrawPath(lapiz, path);
            }
        }

        using var formato = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };
        using var letra = new SolidBrush(Enabled ? ColorLetra : Color.White);
        g.DrawString(Text, Font, letra, new RectangleF(8, 0, Math.Max(1, Width - 16), Height), formato);
    }
}
