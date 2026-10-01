namespace InfoLibro.Datos;

public static class InfoLibroProgramConfiguration
{
    // Asignada en Program.Main
    public static string ConnectionString { get; set; }

    public static string GetConnectionString() => ConnectionString;
}
