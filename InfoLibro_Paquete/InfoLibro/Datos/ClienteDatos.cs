using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InfoLibro.Modelos;

namespace InfoLibro.Datos;

public static class ClienteDatos
{
    public static List<Cliente> ObtenerTodos()
    {
        using var db = CrearContexto();
        return db.Clientes.OrderBy(c => c.IdCliente).AsNoTracking().ToList();
    }

    public static bool Crear(Cliente cliente, out string error)
    {
        error = null;
        if (!Validar(cliente, out error)) return false;

        using var db = CrearContexto();
        if (db.Clientes.Any(c => c.Correo.ToLower() == cliente.Correo.ToLower()))
        {
            error = "Ya existe un cliente con ese correo.";
            return false;
        }

        try
        {
            db.Clientes.Add(cliente);
            db.SaveChanges();
            return true;
        }
        catch (DbUpdateException ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public static bool Modificar(Cliente cliente, out string error)
    {
        error = null;
        if (cliente == null || cliente.IdCliente <= 0)
        {
            error = "Selecciona un cliente válido para modificar.";
            return false;
        }
        if (!Validar(cliente, out error)) return false;

        using var db = CrearContexto();
        var existente = db.Clientes.Find(cliente.IdCliente);
        if (existente == null)
        {
            error = "El cliente seleccionado no existe.";
            return false;
        }

        if (db.Clientes.Any(c => c.IdCliente != cliente.IdCliente && c.Correo.ToLower() == cliente.Correo.ToLower()))
        {
            error = "Ya existe otro cliente con ese correo.";
            return false;
        }

        existente.Nombre = cliente.Nombre;
        existente.Apellido = cliente.Apellido;
        existente.Telefono = cliente.Telefono;
        existente.Correo = cliente.Correo;
        existente.Direccion = cliente.Direccion;

        try
        {
            db.SaveChanges();
            return true;
        }
        catch (DbUpdateException ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public static bool Eliminar(int idCliente, out string error)
    {
        error = null;
        using var db = CrearContexto();
        var cliente = db.Clientes.Find(idCliente);
        if (cliente == null)
        {
            error = "El cliente seleccionado no existe.";
            return false;
        }

        try
        {
            db.Clientes.Remove(cliente);
            db.SaveChanges();
            return true;
        }
        catch (DbUpdateException ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    static bool Validar(Cliente cliente, out string error)
    {
        error = null;
        if (cliente == null || string.IsNullOrWhiteSpace(cliente.Nombre) ||
            string.IsNullOrWhiteSpace(cliente.Apellido) || string.IsNullOrWhiteSpace(cliente.Telefono) ||
            string.IsNullOrWhiteSpace(cliente.Correo) || string.IsNullOrWhiteSpace(cliente.Direccion))
        {
            error = "Todos los campos del cliente son obligatorios.";
            return false;
        }

        if (!Utilidades.Validador.CorreoValido(cliente.Correo))
        {
            error = "Escribe un correo electrónico válido.";
            return false;
        }

        cliente.Nombre = cliente.Nombre.Trim();
        cliente.Apellido = cliente.Apellido.Trim();
        cliente.Telefono = cliente.Telefono.Trim();
        cliente.Correo = cliente.Correo.Trim();
        cliente.Direccion = cliente.Direccion.Trim();
        return true;
    }

    static InfoLibroDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<InfoLibroDbContext>()
            .UseNpgsql(InfoLibroProgramConfiguration.GetConnectionString())
            .Options;
        return new InfoLibroDbContext(options);
    }
}
