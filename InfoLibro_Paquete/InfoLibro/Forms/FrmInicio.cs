using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>
/// Inicio: muestra qué puede hacer el rol actual y permite PROBAR cada permiso.
/// Sirve para demostrar el control de acceso de la Etapa I (video y defensa).
/// Cuando existan los CRUD reales, cada botón real usará Permisos.Verificar igual que aquí.
/// </summary>
public class FrmInicio : Form
{
    readonly Label lblResultado;

    public FrmInicio()
    {
        BackColor = Color.White;
        Rol rol = Sesion.RolActual;

                Label titulo = Tema.Etiqueta("Panel de inicio", Tema.FuenteTexto(22f, FontStyle.Bold), Tema.ColorTexto);
                titulo.SetBounds(40, 20, 700, 48);

                Label sub = Tema.Etiqueta(
                    $"Sesión iniciada como \"{Sesion.UsuarioActual?.NombreUsuario ?? ""}\" con el rol {rol?.NombreRol ?? "Sin Rol"}. Estos son tus permisos:",
                    Tema.FuenteTexto(11f), Tema.ColorTextoSuave);
        sub.SetBounds(40, 72, 900, 28);

        Controls.Add(titulo);
        Controls.Add(sub);

        var filas = new (Accion accion, string nombre, string descripcion, string verbo)[]
        {
            (Accion.Consultar, "Consultar", "Ver la información del sistema", "consultar información"),
            (Accion.Agregar, "Agregar", "Registrar elementos nuevos", "agregar elementos"),
            (Accion.Modificar, "Modificar", "Editar elementos existentes", "modificar elementos"),
            (Accion.Eliminar, "Eliminar", "Borrar elementos", "eliminar elementos"),
            (Accion.Administrar, "Administrar", "Crear usuarios y roles nuevos", "crear usuarios y roles"),
        };

        int y = 116;
        foreach (var f in filas)
        {
            bool permitido = Permisos.Tiene(f.accion);

            var fila = new Panel { Bounds = new Rectangle(40, y, 780, 56), BackColor = Tema.FondoSuave };

            Label lblNombre = Tema.Etiqueta(f.nombre, Tema.FuenteTexto(12f, FontStyle.Bold), Tema.ColorTexto);
            lblNombre.SetBounds(18, 0, 165, 56);
            lblNombre.TextAlign = ContentAlignment.MiddleLeft;

            Label lblDesc = Tema.Etiqueta(f.descripcion, Tema.FuenteTexto(10.5f), Tema.ColorTextoSuave);
            lblDesc.SetBounds(190, 0, 275, 56);
            lblDesc.TextAlign = ContentAlignment.MiddleLeft;

            Label lblEstado = Tema.Etiqueta(permitido ? "PERMITIDO" : "SIN PERMISO",
                Tema.FuenteTexto(10.5f, FontStyle.Bold), permitido ? Tema.Verde : Tema.Rojo);
            lblEstado.SetBounds(470, 0, 150, 56);
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;

            var boton = new BotonRedondo
            {
                Text = "Probar",
                Size = new Size(110, 36),
                Location = new Point(650, 10),
                Font = Tema.FuenteTexto(10.5f, FontStyle.Bold)
            };
            Accion accion = f.accion;
            string verbo = f.verbo;
            boton.Click += (s, e) => Probar(accion, verbo);

            fila.Controls.Add(lblNombre);
            fila.Controls.Add(lblDesc);
            fila.Controls.Add(lblEstado);
            fila.Controls.Add(boton);
            Controls.Add(fila);
            y += 64;
        }

        lblResultado = Tema.Etiqueta("Pulsa \"Probar\" para comprobar un permiso.", Tema.FuenteTexto(12f, FontStyle.Bold), Tema.ColorTextoSuave);
        lblResultado.SetBounds(40, y + 8, 780, 32);
        Controls.Add(lblResultado);
    }

    void Probar(Accion accion, string verbo)
    {
        bool ok = Permisos.Verificar(accion, verbo);   // si no tiene permiso, muestra el MessageBox
        lblResultado.ForeColor = ok ? Tema.Verde : Tema.Rojo;
        lblResultado.Text = ok
            ? $"Acción permitida: tu rol puede {verbo}."
            : $"Acción bloqueada: tu rol no puede {verbo}.";
    }
}
