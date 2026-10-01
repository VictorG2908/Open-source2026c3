using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Datos;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Gestión de usuarios (solo Administrador): crear, restablecer contraseña y eliminar.</summary>
public class FrmUsuarios : Form
{
    readonly DataGridView grid = new DataGridView();
    readonly CampoRedondo txtUsuario = new CampoRedondo { Placeholder = "Ej: arnaldo.abreu", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtNombre = new CampoRedondo { Placeholder = "Arnaldo abreu", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtCorreo = new CampoRedondo { Placeholder = "arnaldo.abreu@ejemplo.com", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtClave = new CampoRedondo { Placeholder = "Mínimo 6 caracteres", EsClave = true, Height = 40, ColorBorde = Tema.Naranja };
    readonly ComboBox cmbRol = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Font = Tema.FuenteCampo(11f), BackColor = Color.White };

    public FrmUsuarios()
    {
        BackColor = Color.White;
        txtUsuario.Caja.MaxLength = 20;
        txtNombre.Caja.MaxLength = 100;
        txtCorreo.Caja.MaxLength = 120;
        txtClave.Caja.MaxLength = 50;

        // --- Título ---
        var pnlTitulo = new Panel { Dock = DockStyle.Top, Height = 90 };
        Label titulo = Tema.Etiqueta("Usuarios", Tema.FuenteTexto(22f, FontStyle.Bold), Tema.ColorTexto);
        titulo.SetBounds(40, 12, 600, 46);
        Label sub = Tema.Etiqueta("Solo el Administrador puede crear usuarios y restablecer contraseñas.", Tema.FuenteTexto(10.5f), Tema.ColorTextoSuave);
        sub.SetBounds(40, 58, 800, 24);
        pnlTitulo.Controls.Add(titulo);
        pnlTitulo.Controls.Add(sub);

        // --- Tabla ---
        Tema.EstiloTabla(grid);
        grid.Dock = DockStyle.Fill;
        grid.Columns.Add("id", "Id");
        grid.Columns.Add("usuario", "Usuario");
        grid.Columns.Add("nombre", "Nombre completo");
        grid.Columns.Add("correo", "Correo");
        grid.Columns.Add("rol", "Rol");
        grid.Columns.Add("estado", "Estado");
        grid.Columns["id"].Visible = false;

        var pnlTabla = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40, 0, 16, 24) };
        pnlTabla.Controls.Add(grid);

        // --- Panel de formulario (derecha) ---
        var derecha = new Panel { Dock = DockStyle.Right, Width = 370, AutoScroll = true };
        int y = 6;
        void Fila(string etiqueta, Control campo)
        {
            Label l = Tema.Etiqueta(etiqueta, Tema.FuenteTexto(10f, FontStyle.Bold), Tema.ColorTextoSuave);
            l.SetBounds(24, y, 320, 20);
            campo.SetBounds(24, y + 22, 320, campo.Height);
            derecha.Controls.Add(l);
            derecha.Controls.Add(campo);
            y += 68;
        }
        Fila("Usuario", txtUsuario);
        Fila("Nombre completo", txtNombre);
        Fila("Correo", txtCorreo);
        Fila("Contraseña", txtClave);
        Fila("Rol", cmbRol);

        var btnCrear = new BotonRedondo { Text = "Crear usuario", Bounds = new Rectangle(24, y + 4, 320, 42) };
        var btnClave = new BotonRedondo
        {
            Text = "Restablecer contraseña",
            Bounds = new Rectangle(24, y + 54, 320, 42),
            ColorNormal = Color.White,
            ColorHover = Tema.NaranjaClaro,
            ColorLetra = Tema.NaranjaOscuro,
            ColorBorde = Tema.Naranja
        };
        var btnEliminar = new BotonRedondo
        {
            Text = "Eliminar usuario",
            Bounds = new Rectangle(24, y + 104, 320, 42),
            ColorNormal = Tema.Rojo,
            ColorHover = Color.FromArgb(200, 40, 36)
        };
        var nota = Tema.Etiqueta("Para restablecer: selecciona un usuario de la tabla y escribe la nueva contraseña arriba.",
            Tema.FuenteTexto(9f), Tema.ColorTextoSuave);
        nota.SetBounds(24, y + 154, 320, 40);

        derecha.Controls.Add(btnCrear);
        derecha.Controls.Add(btnClave);
        derecha.Controls.Add(btnEliminar);
        derecha.Controls.Add(nota);

        Controls.Add(pnlTabla);   // el que llena va primero
        Controls.Add(derecha);
        Controls.Add(pnlTitulo);

        foreach (Rol r in RolDatos.ObtenerTodos()) cmbRol.Items.Add(r);
        if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;

        btnCrear.Click += (s, e) => CrearUsuario();
        btnClave.Click += (s, e) => RestablecerClave();
        btnEliminar.Click += (s, e) => EliminarUsuario();

        CargarUsuarios();
    }

    void CargarUsuarios()
    {
        try
        {
            grid.Rows.Clear();
            foreach (Usuario u in UsuarioDatos.ObtenerTodos())
            {
                grid.Rows.Add(u.IdUsuario, u.NombreUsuario, u.NombreCompleto, u.Correo,
                    u.Rol?.NombreRol, u.Activo ? "Activo" : "Inactivo");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudieron leer los usuarios de la base de datos.\n\nDetalle: " + ex.Message,
                "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    int? IdSeleccionado()
    {
        if (grid.CurrentRow == null) return null;
        return Convert.ToInt32(grid.CurrentRow.Cells["id"].Value);
    }

    void CrearUsuario()
    {
        if (!Permisos.Verificar(Accion.Administrar, "crear usuarios")) return;

        string usuario = txtUsuario.Valor.Trim();
        string nombre = txtNombre.Valor.Trim();
        string correo = txtCorreo.Valor.Trim();
        string clave = txtClave.Valor;

        if (Validador.Vacio(usuario) || Validador.Vacio(nombre) || Validador.Vacio(correo) ||
            Validador.Vacio(clave) || cmbRol.SelectedItem == null)
        {
            MessageBox.Show("Completa todos los campos antes de continuar.", "Campos vacíos",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!Validador.UsuarioValido(usuario))
        {
            MessageBox.Show("El usuario debe tener entre 4 y 20 caracteres (letras, números, punto o guion bajo).",
                "Usuario no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!Validador.CorreoValido(correo))
        {
            MessageBox.Show("Escribe un correo válido (ejemplo: nombre@correo.com).", "Correo no válido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!Validador.ClaveValida(clave))
        {
            MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Contraseña no válida",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var nuevo = new Usuario
        {
            NombreUsuario = usuario,
            NombreCompleto = nombre,
            Correo = correo,
            IdRol = ((Rol)cmbRol.SelectedItem).IdRol
        };

        try
        {
            if (UsuarioDatos.Crear(nuevo, clave, out string error))
            {
                txtUsuario.Valor = "";
                txtNombre.Valor = "";
                txtCorreo.Valor = "";
                txtClave.Valor = "";
                CargarUsuarios();
                MessageBox.Show("Usuario creado correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(error, "No se pudo crear el usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    void RestablecerClave()
    {
        if (!Permisos.Verificar(Accion.Administrar, "restablecer contraseñas")) return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario de la tabla.", "Sin selección",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!Validador.ClaveValida(txtClave.Valor))
        {
            MessageBox.Show("Escribe la nueva contraseña (mínimo 6 caracteres) en el campo Contraseña.",
                "Contraseña no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (UsuarioDatos.RestablecerClave(id.Value, txtClave.Valor))
            {
                txtClave.Valor = "";
                MessageBox.Show("Contraseña restablecida. Si el usuario estaba bloqueado, ya puede entrar.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    void EliminarUsuario()
    {
        if (!Permisos.Verificar(Accion.Eliminar, "eliminar usuarios")) return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario de la tabla.", "Sin selección",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string nombre = Convert.ToString(grid.CurrentRow.Cells["usuario"].Value);
        if (MessageBox.Show($"¿Eliminar al usuario \"{nombre}\"? Esta acción no se puede deshacer.",
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            if (UsuarioDatos.Eliminar(id.Value, out string error)) CargarUsuarios();
            else MessageBox.Show(error, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
