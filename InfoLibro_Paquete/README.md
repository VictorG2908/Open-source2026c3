InfoLibro - Instrucciones para compartir y configurar
===============================================

Resumen
-------
Este repositorio contiene la aplicación InfoLibro (solución Visual Studio) y el script de la base de datos.
Incluye instrucciones para que otro desarrollador pueda reproducir el entorno local y ejecutar la aplicación.

Contenido del paquete
---------------------
- InfoLibro.sln y la carpeta InfoLibro/ con todo el código fuente.
- InfoLibro_BaseDeDatos.sql (script SQL para crear la base de datos y datos de ejemplo).

Requisitos mínimos
------------------
- Visual Studio 2022/2026 o equivalente capaz de abrir soluciones .NET 8.
- .NET 8 SDK instalado.
- SQL Server o LocalDB (recomendado: LocalDB que viene con Visual Studio) o SQL Server Express.
- SQL Server Management Studio (SSMS) opcional para ejecutar scripts y restaurar backups.

Configurar la base de datos (opciones)
--------------------------------------
1) Usar el script SQL (recomendado, fácil y ligero)
   - Abrir SSMS y conectarse a la instancia local (ej. (localdb)\\MSSQLLocalDB o .\\SQLEXPRESS).
   - File -> Open -> seleccionar InfoLibro_BaseDeDatos.sql -> Ejecutar.
   - Verificar que existe la base de datos InfoLibro y las tablas.

2) Restaurar un backup (.bak) (si prefieres réplica exacta)
   - En SSMS: Right click Databases -> Restore Database -> Device -> seleccionar InfoLibro.bak -> Restaurar.

3) Usar los ficheros .mdf/.ldf (Attach) — menos recomendable por permisos y bloqueo.

Cadenas de conexión recomendadas
--------------------------------
En este proyecto la conexión se gestiona en: InfoLibro/Datos/Conexion.cs
El código intenta detectar automáticamente conexiones comunes (LocalDB, SQLEXPRESS, localhost). Si necesitas forzar una, edita la propiedad Conexion.CadenaConexion.

Ejemplos:
- LocalDB (recomendado para desarrollo):
  Server=(localdb)\\MSSQLLocalDB;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;

- SQL Server Express:
  Server=.\\SQLEXPRESS;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;

- SQL Server (instancia por defecto):
  Server=localhost;Database=InfoLibro;Integrated Security=True;TrustServerCertificate=True;

Si usas autenticación SQL (usuario/contraseña):
  Server=localhost;Database=InfoLibro;User Id=sa;Password=TuClave;TrustServerCertificate=True;

Ejecutar la aplicación
----------------------
1. Asegúrate de que la base de datos InfoLibro existe (ver pasos anteriores).
2. Abrir InfoLibro.sln en Visual Studio.
3. Compilar y ejecutar. Si la cadena no coincide, edita InfoLibro/Datos/Conexion.cs para usar la cadena adecuada.

Comprobaciones útiles
---------------------
- LocalDB: abre PowerShell y ejecuta: sqllocaldb info
- Verificar servicio SQL: services.msc -> buscar "SQL Server (INSTANCIA)" -> iniciado
- Probar puerto: Test-NetConnection -ComputerName localhost -Port 1433

Cómo compartir el proyecto con un compañero (opciones)
------------------------------------------------------
A) Repositorio Git (mejor práctica)
   - Subir el código a GitHub/GitLab.
   - Incluir InfoLibro_BaseDeDatos.sql dentro de una carpeta /Database.
   - Añadir .gitignore (excluir bin/, obj/, user secrets).
   - Añadir README con pasos (este fichero).

B) ZIP con proyecto + script SQL (rápido y fiable)
   - Comprimir todo el directorio del proyecto (sin bin/obj si quieres ahorrar peso) y adjuntar InfoLibro_BaseDeDatos.sql.
   - Incluir instrucciones en el README.

C) Backup (.bak)
   - Incluir InfoLibro.bak para restaurar la BD exactamente.

D) Docker (opcional, para reproducibilidad)
   - Proveer docker-compose con una imagen mcr.microsoft.com/mssql/server y script de inicialización.

Buenas prácticas
---------------
- No subir contraseñas ni credenciales al repositorio.
- Documentar la versión de SQL Server usada si hay dependencias específicas.
- Incluir instrucciones claras para usar LocalDB (es la opción más sencilla para desarrolladores Windows).

Soporte adicional
-----------------
Si quieres, puedo generar:
- Un README.txt/README.md más detallado adaptado a vuestro flujo.
- Un .zip listo para compartir (preparado para subir).
- Un docker-compose.yml que levante SQL Server y ejecute el script de inicialización.

Indica cuál prefieres y lo creo.
