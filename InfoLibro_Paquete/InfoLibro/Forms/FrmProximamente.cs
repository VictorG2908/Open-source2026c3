using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Pantalla temporal para los módulos que se construyen en etapas siguientes.</summary>
public class FrmProximamente : Form
{
    public FrmProximamente(string nombreModulo)
    {
        BackColor = Color.White;

        var lblTitulo = Tema.Etiqueta(nombreModulo, Tema.FuenteTitulo(40f), Tema.Naranja);
        lblTitulo.Dock = DockStyle.Top;
        lblTitulo.Height = 180;
        lblTitulo.TextAlign = ContentAlignment.BottomCenter;

        var lblTexto = Tema.Etiqueta("Este módulo se desarrollará en las siguientes etapas del proyecto.",
            Tema.FuenteTexto(13f), Tema.ColorTextoSuave);
        lblTexto.Dock = DockStyle.Top;
        lblTexto.Height = 60;
        lblTexto.TextAlign = ContentAlignment.MiddleCenter;

        Controls.Add(lblTexto);   // se añade primero y queda debajo
        Controls.Add(lblTitulo);
    }
}
