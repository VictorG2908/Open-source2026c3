namespace InfoLibro.Modelos;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string NombreCompleto { get; set; }
    public string Correo { get; set; }
    public string ClaveHash { get; set; }
    public string Salt { get; set; }
    public int IdRol { get; set; }
    public Rol Rol { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>Primer nombre, para el saludo "Hi, Pedro!".</summary>
    public string PrimerNombre
    {
        get
        {
            string n = (NombreCompleto ?? NombreUsuario ?? "").Trim();
            int i = n.IndexOf(' ');
            return i > 0 ? n.Substring(0, i) : n;
        }
    }
}
