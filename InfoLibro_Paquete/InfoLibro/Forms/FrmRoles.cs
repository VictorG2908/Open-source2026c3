using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Datos;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Gestión de roles (solo Administrador): ver los roles y crear roles nuevos.</summary>
public class FrmRoles : Form
{
    readonly DataGridView grid = new DataGridView();
    readonly CampoRedondo txtNombre = new CampoRedondo { Placeholder = "Ej: Auditor", Height = 40, ColorBorde = Tema.Naranja };
    readonly CheckBox chkConsultar = Casilla("Consultar", true);
    readonly CheckBox chkAgregar = Casilla("Agregar", false);
    readonly CheckBox chkModificar = Casilla("Modificar", false);
    readonly CheckBox chkEliminar = Casilla("Eliminar", false);
    readonly CheckBox chkAdministrar = Casilla("Administrar (crear usuarios y roles)", false);

    static CheckBox Casilla(string texto, bool marcado) => new CheckBox
    {
        Text = texto,
        Checked = marcado,
        AutoSize = true,
        Font = Tema.FuenteCampo(11f),
        ForeColor = Tema.ColorTexto
    };

    public FrmRoles()
    {
        BackColor = Color.White;
        txtNombre.Caja.MaxLength = 50;

        // --- Título ---
        var pnlTitulo = new Panel { Dock = DockStyle.Top, Height = 90 };
        Label titulo = Tema.Etiqueta("Roles y permisos", Tema.FuenteTexto(22f, FontStyle.Bold), Tema.ColorTexto);
        titulo.SetBounds(40, 12, 600, 46);
        Label sub = Tema.Etiqueta("Los tres roles base vienen de la asignación. El Administrador puede crear roles nuevos.",
            Tema.FuenteTexto(10.5f), Tema.ColorTextoSuave);
        sub.SetBounds(40, 58, 900, 24);
        pnlTitulo.Controls.Add(titulo);
        pnlTitulo.Controls.Add(sub);

        // --- Tabla ---
        Tema.EstiloTabla(grid);
        grid.Dock = DockStyle.Fill;
        grid.Columns.Add("id", "Id");
        grid.Columns.Add("rol", "Rol");
        grid.Columns.Add("consultar", "Consultar");
        grid.Columns.Add("agregar", "Agregar");
        grid.Columns.Add("modificar", "Modificar");
        grid.Columns.Add("eliminar", "Eliminar");
        grid.Columns.Add("administrar", "Administrar");
        grid.Columns["id"].Visible = false;

        var pnlTabla = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40, 0, 16, 24) };
        pnlTabla.Controls.Add(grid);

        // --- Panel de formulario (derecha) ---
        var derecha = new Panel { Dock = DockStyle.Right, Width = 370, AutoScroll = true };

        Label lNombre = Tema.Etiqueta("Nombre del rol nuevo", Tema.FuenteTexto(10f, FontStyle.Bold), Tema.ColorTextoSuave);
        lNombre.SetBounds(24, 6, 320, 20);
        txtNombre.SetBounds(24, 28, 320, 40);

        Label lPermisos = Tema.Etiqueta("Permisos", Tema.FuenteTexto(10f, FontStyle.Bold), Tema.ColorTextoSuave);
        lPermisos.SetBounds(24, 84, 320, 20);

        int y = 110;
        foreach (CheckBox c in new[] { chkConsultar, chkAgregar, chkModificar, chkEliminar, chkAdministrar })
        {
            c.Location = new Point(28, y);
            derecha.Controls.Add(c);
            y += 34;
        }

        var btnCrear = new BotonRedondo { Text = "Crear rol", Bounds = new Rectangle(24, y + 16, 320, 42) };

        derecha.Controls.Add(lNombre);
        derecha.Controls.Add(txtNombre);
        derecha.Controls.Add(lPermisos);
        derecha.Controls.Add(btnCrear);

        Controls.Add(pnlTabla);   // el que llena va primero
        Controls.Add(derecha);
        Controls.Add(pnlTitulo);

        btnCrear.Click += (s, e) => CrearRol();
        CargarRoles();
    }

    static string SiNo(bool valor) => valor ? "Sí" : "No";

    void CargarRoles()
    {
        try
        {
            grid.Rows.Clear();
            foreach (Rol r in RolDatos.ObtenerTodos())
            {
                grid.Rows.Add(r.IdRol, r.NombreRol, SiNo(r.PuedeConsultar), SiNo(r.PuedeAgregar),
                    SiNo(r.PuedeModificar), SiNo(r.PuedeEliminar), SiNo(r.PuedeAdministrar));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudieron leer los roles de la base de datos.\n\nDetalle: " + ex.Message,
                "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    void CrearRol()
    {
        if (!Permisos.Verificar(Accion.Administrar, "crear roles")) return;

        string nombre = txtNombre.Valor.Trim();
        if (Validador.Vacio(nombre))
        {
            MessageBox.Show("Escribe un nombre para el rol.", "Campo vacío",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var rol = new Rol
        {
            NombreRol = nombre,
            PuedeConsultar = chkConsultar.Checked,
            PuedeAgregar = chkAgregar.Checked,
            PuedeModificar = chkModificar.Checked,
            PuedeEliminar = chkEliminar.Checked,
            PuedeAdministrar = chkAdministrar.Checked
        };

        try
        {
            if (RolDatos.Crear(rol, out string error))
            {
                txtNombre.Valor = "";
                CargarRoles();
                MessageBox.Show("Rol creado correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(error, "No se pudo crear el rol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
