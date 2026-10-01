using System.Drawing;
using System.Windows.Forms;
namespace InfoLibro.Utilidades;

/// <summary>Panel con doble búfer para que no parpadee al dibujar encima.</summary>
public class PanelDoble : Panel
{
    public PanelDoble()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }
}
