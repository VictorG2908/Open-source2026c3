namespace InfoLibro.Modelos;

/// <summary>Acciones que el profesor define para los roles.</summary>
public enum Accion
{
    Consultar,
    Agregar,
    Modificar,
    Eliminar,
    Administrar   // crear usuarios y nuevos roles
}

public class Rol
{
    public int IdRol { get; set; }
    public string NombreRol { get; set; }
    public bool PuedeConsultar { get; set; }
    public bool PuedeAgregar { get; set; }
    public bool PuedeModificar { get; set; }
    public bool PuedeEliminar { get; set; }
    public bool PuedeAdministrar { get; set; }

    public bool Permite(Accion accion)
    {
        switch (accion)
        {
            case Accion.Consultar: return PuedeConsultar;
            case Accion.Agregar: return PuedeAgregar;
            case Accion.Modificar: return PuedeModificar;
            case Accion.Eliminar: return PuedeEliminar;
            case Accion.Administrar: return PuedeAdministrar;
            default: return false;
        }
    }

    public override string ToString() => NombreRol;
}
