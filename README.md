# Centro Médico - Cierre de consulta

Base del proyecto final, Dominio 4. Incluye el cierre completo y pantallas para consultar sus resultados.

1. Ejecutar `database/01_CentroMedico.sql` en SQL Server Management Studio.
2. Abrir `CentroMedico.sln` en Visual Studio con .NET 8 y la carga de trabajo de escritorio .NET.
3. Revisar `src/CentroMedico.UI/App.config`. La conexión usa tu instancia `DESKTOP-8BDRFGC\LEGIONSQL` y la base `CentroMedico`.
4. Establecer `CentroMedico.UI` como proyecto de inicio. Restaurar los paquetes NuGet y ejecutar.
5. Seleccionar una cita, completar el historial, agregar medicamentos e insumos, y presionar **Cerrar consulta**.

## Proyectos

| Proyecto | Contenido | Referencias |
| --- | --- | --- |
| Domain | Entidades, reglas principales e interfaz de repositorio | Ningún otro proyecto ni paquete |
| Application | DTO, normalización, validación y caso de uso | Domain |
| Infrastructure | Repositorio con ADO.NET y SqlTransaction | Domain |
| UI | ViewModels, RelayCommand, INotifyPropertyChanged, ObservableCollection y XAML | Application, Domain; Infrastructure solo en App para inyección |

`App.xaml.cs` conecta las dependencias por constructor. `Views/MainWindow.xaml` no tiene archivo de código detrás. La navegación usa `vistaActual` y `DataTemplates`.

El cierre ejecuta historial, receta con detalles, orden de cobro, descuento de stock, registro de consumos y cambio de estado de la cita. Todos los comandos participan en una sola conexión/transacción, que termina mediante `Commit()` o `Rollback()`, siguiendo el contrato de [SqlTransaction de Microsoft](https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqltransaction).

## Alcance de esta entrega

- Agenda: selección y consulta de citas pendientes incluidas en el seed.
- Historial y recetas: registro durante el cierre y consulta posterior; las recetas permiten varios medicamentos.
- Facturación: emisión y listado de órdenes de cobro pendientes. El importe es la tarifa de la cita; los insumos se consideran incluidos en ella.
- Inventario: selección, comprobación de existencias y descuento con trazabilidad por historial.
- La captura de nuevas citas/pacientes, reprogramación, cobro efectivo e integración tributaria quedan fuera de este bloque de desarrollo.

Los pacientes, el médico y los insumos del seed son ficticios. Los datos clínicos de las pruebas son textos de demostración.

## Fondos y presentación de WPF

La ventana incluye una cabecera con fotografía médica, colores azules y paneles claros. Agenda / Cierre muestra `FondoConsulta.png`; Historial, Recetas y Facturación muestran `FondoConsultorio.png`. El cambio de fondo usa DataTemplates en XAML.

Las dos imágenes están en `src/CentroMedico.UI/Assets/` y se incluyen como recursos de WPF en `CentroMedico.UI.csproj`. Se cargan desde la aplicación compilada, sin conexión a internet ni rutas absolutas de tu computadora.

Para incorporar este diseño en una copia anterior del proyecto, actualizar únicamente:

1. `src/CentroMedico.UI/Views/MainWindow.xaml`.
2. `src/CentroMedico.UI/CentroMedico.UI.csproj` (el ItemGroup que incluye `Assets\*.png` como Resource).
3. Copiar la carpeta `src/CentroMedico.UI/Assets/` con sus dos imágenes PNG.

Se conservan los archivos C#, los scripts SQL, la configuración de conexión, los bindings existentes, los comandos y las cuatro capas. Los fondos no requieren ejecutar de nuevo el script de la base de datos. La prueba de fallo continúa en Agenda / Cierre, siguiendo `PRUEBAS.md`.

## Verificación realizada

- Los archivos C# de las cuatro capas fueron compilados con Roslyn y las referencias de .NET 8, WPF y los paquetes configurados: sin errores ni advertencias.
- Se verificó la sintaxis XML de XAML, App.config y los proyectos.
- Se verificó la sintaxis T-SQL de los scripts y consultas con el analizador de Microsoft.
- Se ejecutaron comprobaciones de validación y de copia de los datos enviados al repositorio, sin conexión a una base de datos.
- En la actualización visual se comprobó el XML del XAML y del proyecto, la existencia de las dos imágenes y sus rutas como Resource, la conservación de los bindings y comandos, y la igualdad de todos los archivos C# y SQL con la entrega anterior.
- La compilación de BAML y la ejecución visual de WPF requieren comprobación en Windows. No se ejecutó el cierre contra una instancia de SQL Server en este entorno. Seguir `PRUEBAS.md` para verificar Commit, Rollback y concurrencia en tu equipo.
