using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using InfoLibro.Datos;
using InfoLibro.Seguridad;
using InfoLibro.Utilidades;

namespace InfoLibro.Forms;

/// <summary>Pantalla de login (Etapa I). Diseño basado en el Figma de InfoLibro.</summary>
public class FrmLogin : Form
{
    readonly Image fondo = Recurso.Imagen("fondo.png");
    Bitmap fondoCache;

    readonly Label lblLogo;
    readonly Label lblEstado;
    readonly Label lblAyuda;
    readonly CampoRedondo txtUsuario;
    readonly CampoRedondo txtClave;
    readonly BotonRedondo btnIniciar;
    readonly BotonRedondo btnSalir;

    public FrmLogin()
    {
        Text = "InfoLibro - Iniciar sesión";
        ClientSize = new Size(1000, 640);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        BackColor = Tema.Cafe;

        int cx = ClientSize.Width / 2;

        // Logo (texto con fuente gótica)
        lblLogo = Tema.Etiqueta("InfoLibro", Tema.FuenteTitulo(60f), Color.White);
        lblLogo.SetBounds(0, 80, ClientSize.Width, 120);
        lblLogo.TextAlign = ContentAlignment.MiddleCenter;

        // Campos
        txtUsuario = new CampoRedondo { Placeholder = "Usuario", Bounds = new Rectangle(cx - 200, 250, 400, 50) };
        txtClave = new CampoRedondo { Placeholder = "Contraseña", EsClave = true, Bounds = new Rectangle(cx - 200, 314, 400, 50) };
        txtUsuario.Caja.MaxLength = 30;
        txtClave.Caja.MaxLength = 50;

        // Mensaje de intentos restantes
        lblEstado = Tema.Etiqueta("", Tema.FuenteTexto(10.5f, FontStyle.Bold), Color.FromArgb(255, 205, 185));
        lblEstado.SetBounds(cx - 200, 374, 400, 28);
        lblEstado.TextAlign = ContentAlignment.MiddleCenter;

        // Botones
        btnIniciar = new BotonRedondo { Text = "Iniciar sesión", Bounds = new Rectangle(cx - 115, 414, 230, 50) };
        btnSalir = new BotonRedondo
        {
            Text = "Salir",
            Bounds = new Rectangle(cx - 115, 476, 230, 50),
            ColorNormal = Color.FromArgb(40, 255, 255, 255),
            ColorHover = Color.FromArgb(90, 255, 255, 255),
            ColorBorde = Color.White
        };

        lblAyuda = Tema.Etiqueta("¿Necesitas acceso? Contacta al administrador.", Tema.FuenteTexto(10f), Color.White);
        lblAyuda.SetBounds(0, 560, ClientSize.Width, 28);
        lblAyuda.TextAlign = ContentAlignment.MiddleCenter;

        Controls.Add(lblLogo);
        Controls.Add(txtUsuario);
        Controls.Add(txtClave);
        Controls.Add(lblEstado);
        Controls.Add(btnIniciar);
        Controls.Add(btnSalir);
        Controls.Add(lblAyuda);

        // Eventos
        btnIniciar.Click += (s, e) => IniciarSesion();
        btnSalir.Click += (s, e) => DialogResult = DialogResult.Cancel;
        txtUsuario.Caja.KeyDown += TeclaEnter;
        txtClave.Caja.KeyDown += TeclaEnter;
        Shown += (s, e) => txtUsuario.Caja.Focus();
    }

    void TeclaEnter(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        if (ReferenceEquals(sender, txtUsuario.Caja)) txtClave.Caja.Focus();
        else IniciarSesion();
    }

    void IniciarSesion()
    {
        lblEstado.Text = "";
        string usuario = txtUsuario.Valor.Trim();
        string clave = txtClave.Valor;

        // 1) Validar campos vacíos
        if (Validador.Vacio(usuario) || Validador.Vacio(clave))
        {
            MessageBox.Show("Escribe tu usuario y tu contraseña para continuar.",
                "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            (Validador.Vacio(usuario) ? txtUsuario : txtClave).Caja.Focus();
            return;
        }

        // 2) Validar credenciales (Datos/UsuarioDatos.cs)
        ResultadoLogin r;
        try
        {
            r = UsuarioDatos.ValidarCredenciales(usuario, clave);
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo comunicar con la base de datos.\n\nDetalle: " + ex.Message,
                "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (r.Exito)
        {
            Sesion.Iniciar(r.Usuario);
            DialogResult = DialogResult.OK;   // cierra el login y abre el menú principal
            return;
        }

        // 3) Acceso incorrecto: se bloquea la entrada y se informa
        txtClave.Valor = "";
        if (r.Bloqueado)
        {
            lblEstado.Text = "Acceso bloqueado temporalmente.";
            MessageBox.Show(r.Mensaje, "Acceso bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        else
        {
            lblEstado.Text = $"Intentos restantes: {r.IntentosRestantes}";
            MessageBox.Show(r.Mensaje, "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        txtClave.Caja.Focus();
    }

    // ---- Fondo: biblioteca + velo oscuro, dibujado una vez y reutilizado ----
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (ClientSize.Width < 2 || ClientSize.Height < 2) return;

        if (fondoCache == null || fondoCache.Size != ClientSize)
        {
            fondoCache?.Dispose();
            fondoCache = CrearFondo();
        }
        e.Graphics.DrawImageUnscaled(fondoCache, 0, 0);
    }

    Bitmap CrearFondo()
    {
        var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
        using (var g = Graphics.FromImage(bmp))
        {
            var r = new Rectangle(Point.Empty, ClientSize);
            Tema.DibujarCubierto(g, fondo, r, 0.6f);
            using var velo = new LinearGradientBrush(r, Color.FromArgb(140, 20, 10, 5), Color.FromArgb(215, 20, 10, 5), 90f);
            g.FillRectangle(velo, r);
        }
        return bmp;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) fondoCache?.Dispose();
        base.Dispose(disposing);
    }
}
