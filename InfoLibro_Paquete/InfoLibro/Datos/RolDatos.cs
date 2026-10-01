using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InfoLibro.Modelos;
using InfoLibro.Seguridad;

namespace InfoLibro.Datos;

/// <summary>Acceso a la tabla Roles usando EF Core.</summary>
public static class RolDatos
{
    public static List<Rol> ObtenerTodos()
    {
        using var db = CreateContext();
        return db.Roles.OrderBy(r => r.IdRol).ToList();
    }

    public static Rol ObtenerPorId(int idRol)
    {
        using var db = CreateContext();
        return db.Roles.Find(idRol);
    }

    public static bool Crear(Rol rol, out string error)
    {
        Permisos.Exigir(Accion.Administrar, "crear roles");
        error = null;

        if (string.IsNullOrWhiteSpace(rol.NombreRol))
        {
            error = "El nombre del rol es obligatorio.";
            return false;
        }
        string nombre = rol.NombreRol.Trim();

        using var db = CreateContext();
        if (db.Roles.Any(r => r.NombreRol == nombre))
        {
            error = "Ya existe un rol con ese nombre.";
            return false;
        }

        rol.NombreRol = nombre;
        try
        {
            db.Roles.Add(rol);
            db.SaveChanges();
            return true;
        }
        catch (DbUpdateException ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
    }

    static InfoLibroDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InfoLibroDbContext>()
            .UseNpgsql(InfoLibroProgramConfiguration.GetConnectionString())
            .Options;
        return new InfoLibroDbContext(options);
    }
}
