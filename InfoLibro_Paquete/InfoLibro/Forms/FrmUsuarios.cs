using System.Drawing;
using System.Windows.Forms;
using InfoLibro.Datos;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Gestión de usuarios (solo Administrador): crear, leer, actualizar y activar/desactivar.</summary>
public class FrmUsuarios : Form
{
    readonly DataGridView grid = new DataGridView();
    readonly CampoRedondo txtUsuario = new CampoRedondo { Placeholder = "Ej: arnaldo.abreu", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtNombre = new CampoRedondo { Placeholder = "Arnaldo abreu", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtCorreo = new CampoRedondo { Placeholder = "arnaldo.abreu@ejemplo.com", Height = 40, ColorBorde = Tema.Naranja };
    readonly CampoRedondo txtClave = new CampoRedondo { Placeholder = "Mínimo 6 caracteres", EsClave = true, Height = 40, ColorBorde = Tema.Naranja };
    readonly ComboBox cmbRol = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Font = Tema.FuenteCampo(11f), BackColor = Color.White };
    readonly BotonRedondo btnEstado = new BotonRedondo
    {
        Text = "Desactivar usuario",
        ColorNormal = Tema.NaranjaClaro,
        ColorHover = Tema.Naranja,
        ColorLetra = Tema.ColorTexto,
        ColorBorde = Tema.Naranja
    };

    static string NombreRolParaMostrar(Usuario usuario)
    {
        if (usuario == null)
            return "Usuario";

        var nombreUsuario = (usuario.NombreUsuario ?? string.Empty).Trim();

        if (string.Equals(nombreUsuario, "admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(nombreUsuario, "brianna", StringComparison.OrdinalIgnoreCase))
            return "Administrador";

        if (string.Equals(nombreUsuario, "supervisor", StringComparison.OrdinalIgnoreCase))
            return "Supervisor";

        if (string.Equals(nombreUsuario, "ejecutor", StringComparison.OrdinalIgnoreCase))
            return "Ejecutor";

        if (usuario.Rol != null && !string.IsNullOrWhiteSpace(usuario.Rol.NombreRol))
            return usuario.Rol.NombreRol;

        return "Usuario";
    }

    Rol ObtenerRolSeleccionadoOUsuario()
    {
        if (cmbRol.SelectedItem is Rol rolSeleccionado && rolSeleccionado.IdRol > 0)
            return rolSeleccionado;

        var roles = RolDatos.ObtenerTodos();
        var rolUsuario = roles.FirstOrDefault(r => string.Equals(r.NombreRol, "Usuario", StringComparison.OrdinalIgnoreCase));
        if (rolUsuario != null)
        {
            cmbRol.SelectedItem = rolUsuario;
            return rolUsuario;
        }

        if (roles.Count > 0)
        {
            cmbRol.SelectedIndex = 0;
            return roles[0];
        }

        return null;
    }

    void ActualizarTextoBotonEstado()
    {
        int? id = IdSeleccionado();
        if (id == null)
        {
            btnEstado.Text = "Desactivar usuario";
            return;
        }

        var usuario = UsuarioDatos.ObtenerPorId(id.Value);
        btnEstado.Text = usuario != null && usuario.Activo ? "Desactivar usuario" : "Activar usuario";
    }

    public FrmUsuarios()
    {
        BackColor = Color.White;
        txtUsuario.Caja.MaxLength = 20;
        txtNombre.Caja.MaxLength = 100;
        txtCorreo.Caja.MaxLength = 120;
        txtClave.Caja.MaxLength = 50;

        var pnlTitulo = new Panel { Dock = DockStyle.Top, Height = 90 };
        Label titulo = Tema.Etiqueta("Usuarios", Tema.FuenteTexto(22f, FontStyle.Bold), Tema.ColorTexto);
        titulo.SetBounds(40, 12, 600, 46);
        Label sub = Tema.Etiqueta("Solo el Administrador puede crear, actualizar y gestionar usuarios.", Tema.FuenteTexto(10.5f), Tema.ColorTextoSuave);
        sub.SetBounds(40, 58, 800, 24);
        pnlTitulo.Controls.Add(titulo);
        pnlTitulo.Controls.Add(sub);

        Tema.EstiloTabla(grid);
        grid.Dock = DockStyle.Fill;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.Columns.Add("id", "Id");
        grid.Columns.Add("usuario", "Usuario");
        grid.Columns.Add("nombre", "Nombre completo");
        grid.Columns.Add("correo", "Correo");
        grid.Columns.Add("rol", "Rol");
        grid.Columns.Add("estado", "Estado");
        grid.Columns["id"].Visible = false;

        var pnlTabla = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40, 0, 16, 24) };
        pnlTabla.Controls.Add(grid);

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
        var btnActualizar = new BotonRedondo
        {
            Text = "Actualizar usuario",
            Bounds = new Rectangle(24, y + 54, 320, 42),
            ColorNormal = Color.White,
            ColorHover = Tema.NaranjaClaro,
            ColorLetra = Tema.NaranjaOscuro,
            ColorBorde = Tema.Naranja
        };
        btnEstado.Bounds = new Rectangle(24, y + 104, 320, 42);
        var btnClave = new BotonRedondo
        {
            Text = "Restablecer contraseña",
            Bounds = new Rectangle(24, y + 154, 320, 42),
            ColorNormal = Color.White,
            ColorHover = Tema.NaranjaClaro,
            ColorLetra = Tema.NaranjaOscuro,
            ColorBorde = Tema.Naranja
        };
        var btnEliminar = new BotonRedondo
        {
            Text = "Eliminar usuario",
            Bounds = new Rectangle(24, y + 204, 320, 42),
            ColorNormal = Tema.Rojo,
            ColorHover = Color.FromArgb(200, 40, 36)
        };
        var nota = Tema.Etiqueta("Selecciona un usuario para cargar sus datos y cambiar su estado o rol.", Tema.FuenteTexto(9f), Tema.ColorTextoSuave);
        nota.SetBounds(24, y + 254, 320, 40);

        derecha.Controls.Add(btnCrear);
        derecha.Controls.Add(btnActualizar);
        derecha.Controls.Add(btnEstado);
        derecha.Controls.Add(btnClave);
        derecha.Controls.Add(btnEliminar);
        derecha.Controls.Add(nota);

        Controls.Add(pnlTabla);
        Controls.Add(derecha);
        Controls.Add(pnlTitulo);

        var roles = RolDatos.ObtenerTodos();
        foreach (Rol r in roles)
            cmbRol.Items.Add(r);

        var rolPorDefecto = roles.FirstOrDefault(r => string.Equals(r.NombreRol, "Usuario", StringComparison.OrdinalIgnoreCase));
        if (rolPorDefecto != null)
            cmbRol.SelectedItem = rolPorDefecto;
        else if (cmbRol.Items.Count > 0)
            cmbRol.SelectedIndex = 0;

        grid.SelectionChanged += (s, e) => CargarSeleccionEnFormulario();
        btnCrear.Click += (s, e) => CrearUsuario();
        btnActualizar.Click += (s, e) => ActualizarUsuario();
        btnEstado.Click += (s, e) => ToggleEstadoUsuario();
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
                string nombreRol = NombreRolParaMostrar(u);
                grid.Rows.Add(u.IdUsuario, u.NombreUsuario, u.NombreCompleto, u.Correo,
                    nombreRol, u.Activo ? "Activo" : "Inactivo");
            }
            CargarSeleccionEnFormulario();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudieron leer los usuarios de la base de datos.\n\nDetalle: " + ex.Message,
                "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    void CargarSeleccionEnFormulario()
    {
        int? id = IdSeleccionado();
        if (id == null)
        {
            txtUsuario.Valor = "";
            txtNombre.Valor = "";
            txtCorreo.Valor = "";
            txtClave.Valor = "";
            return;
        }

        var usuario = UsuarioDatos.ObtenerPorId(id.Value);
        if (usuario == null)
            return;

        txtUsuario.Valor = usuario.NombreUsuario;
        txtNombre.Valor = usuario.NombreCompleto;
        txtCorreo.Valor = usuario.Correo;
        txtClave.Valor = "";

        foreach (Rol r in cmbRol.Items)
        {
            if (r.IdRol == usuario.IdRol)
            {
                cmbRol.SelectedItem = r;
                break;
            }
        }

        ActualizarTextoBotonEstado();
    }

    int? IdSeleccionado()
    {
        if (grid.CurrentRow == null)
            return null;
        return Convert.ToInt32(grid.CurrentRow.Cells["id"].Value);
    }

    bool ValidarFormulario(bool requiereClave, out string usuario, out string nombre, out string correo, out string clave, out Rol rol)
    {
        usuario = txtUsuario.Valor.Trim();
        nombre = txtNombre.Valor.Trim();
        correo = txtCorreo.Valor.Trim();
        clave = txtClave.Valor;
        rol = ObtenerRolSeleccionadoOUsuario();

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) ||
            rol == null || (requiereClave && string.IsNullOrWhiteSpace(clave)))
        {
            MessageBox.Show("Completa todos los campos antes de continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!Validador.UsuarioValido(usuario))
        {
            MessageBox.Show("El usuario debe tener entre 4 y 20 caracteres (letras, números, punto o guion bajo).", "Usuario no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!Validador.CorreoValido(correo))
        {
            MessageBox.Show("Escribe un correo válido (ejemplo: nombre@correo.com).", "Correo no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (requiereClave && !Validador.ClaveValida(clave))
        {
            MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Contraseña no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    void CrearUsuario()
    {
        if (!Permisos.Verificar(Accion.Administrar, "crear usuarios"))
            return;

        if (!ValidarFormulario(true, out string usuario, out string nombre, out string correo, out string clave, out Rol rol))
            return;

        var nuevo = new Usuario
        {
            NombreUsuario = usuario,
            NombreCompleto = nombre,
            Correo = correo,
            IdRol = rol.IdRol
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
                MessageBox.Show("Usuario creado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    void ActualizarUsuario()
    {
        if (!Permisos.Verificar(Accion.Administrar, "actualizar usuarios"))
            return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario de la tabla antes de actualizarlo.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidarFormulario(false, out string usuario, out string nombre, out string correo, out _, out Rol rol))
            return;

        var datos = new Usuario
        {
            NombreUsuario = usuario,
            NombreCompleto = nombre,
            Correo = correo,
            IdRol = rol.IdRol,
            Activo = UsuarioDatos.ObtenerPorId(id.Value)?.Activo ?? true
        };

        try
        {
            if (UsuarioDatos.Actualizar(id.Value, datos, out string error))
            {
                CargarUsuarios();
                MessageBox.Show("Usuario actualizado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(error, "No se pudo actualizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    void ToggleEstadoUsuario()
    {
        if (!Permisos.Verificar(Accion.Administrar, "activar o desactivar usuarios"))
            return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario para cambiar su estado.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var usuario = UsuarioDatos.ObtenerPorId(id.Value);
        if (usuario == null)
        {
            MessageBox.Show("No se encontró el usuario seleccionado.", "Usuario no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool nuevoEstado = !usuario.Activo;
        string accion = nuevoEstado ? "activar" : "desactivar";
        if (MessageBox.Show($"¿{char.ToUpper(accion[0])}{accion.Substring(1)} al usuario \"{usuario.NombreUsuario}\"?", "Confirmar estado", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            bool ok = nuevoEstado ? UsuarioDatos.Activar(id.Value, out string errorActivo) : UsuarioDatos.Desactivar(id.Value, out string errorInactivo);
            if (ok)
            {
                CargarUsuarios();
                ActualizarTextoBotonEstado();
                MessageBox.Show($"Usuario {(nuevoEstado ? "activado" : "desactivado")} correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show((nuevoEstado ? "No se pudo activar el usuario." : "No se pudo desactivar el usuario."), "Error de estado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    void RestablecerClave()
    {
        if (!Permisos.Verificar(Accion.Administrar, "restablecer contraseñas"))
            return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario de la tabla.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!Validador.ClaveValida(txtClave.Valor))
        {
            MessageBox.Show("Escribe la nueva contraseña (mínimo 6 caracteres) en el campo Contraseña.", "Contraseña no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (UsuarioDatos.RestablecerClave(id.Value, txtClave.Valor))
            {
                txtClave.Valor = "";
                MessageBox.Show("Contraseña restablecida. Si el usuario estaba bloqueado, ya puede entrar.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Contraseña no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    void EliminarUsuario()
    {
        if (!Permisos.Verificar(Accion.Eliminar, "eliminar usuarios"))
            return;

        int? id = IdSeleccionado();
        if (id == null)
        {
            MessageBox.Show("Selecciona un usuario de la tabla.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string nombre = Convert.ToString(grid.CurrentRow.Cells["usuario"].Value);
        if (MessageBox.Show($"¿Eliminar al usuario \"{nombre}\"? Esta acción no se puede deshacer.", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            if (UsuarioDatos.Eliminar(id.Value, out string error))
                CargarUsuarios();
            else
                MessageBox.Show(error, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (PermisoDenegadoException ex)
        {
            MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
