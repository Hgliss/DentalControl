# MVVM en DentalControl

## Alcance actual

El login y la navegación usan ViewModels de CommunityToolkit.Mvvm. Las pantallas de
pacientes, pagos, reportes y administración conservan sus mensajes de próximamente:
sus ViewModels son puntos de entrada, no módulos funcionales nuevos.

No se modifican entidades, configuraciones de EF Core, migraciones ni reglas de
`AuthenticationService`. La refactorización conserva los tamaños, estilos y textos
existentes. Ampliar el login o rediseñar Inicio será otro cambio.

## Responsabilidades y carpetas

| Ubicación | Responsabilidad |
| --- | --- |
| `Views/` | XAML y comportamiento visual de los controles. |
| `ViewModels/<Funcionalidad>/` | Estado observable y coordinación de acciones. |
| `ViewModels/Navigation/` | Identificadores de pantallas y contrato de construcción. |
| `Services/<Funcionalidad>/` | Operaciones y contratos que consume la presentación. |
| `Infrastructure/` | Adaptadores de presentación: conversores y fábrica de pantallas. |
| `DependencyInjection.cs` | Registro de los componentes de presentación. |
| `Data/` | Contexto, inicialización y configuración de persistencia existentes. |
| `Models/` | Entidades y catálogos existentes. |
| `App.xaml.cs` | Composición, sesión y ciclo de vida de las ventanas. |

`MainWindow` permanece en la raíz para no mover archivos sin necesidad.

Los ViewModels heredan directamente de `ObservableObject`. No hay una base propia
vacía ni repositorios o servicios CRUD genéricos. Las propiedades se escriben con
`SetProperty`; los comandos reutilizan `RelayCommand`. Cuando una pantalla necesite
una operación asíncrona sin datos sensibles, podrá usar `AsyncRelayCommand`, definiendo
también el manejo de errores y la prevención de operaciones simultáneas.

## Login y contraseña

1. La vista enlaza usuario, mensaje, texto del botón y disponibilidad al `LoginViewModel`.
2. Un manejador visual recoge la contraseña y espera `LoginAsync` del ViewModel.
3. El ViewModel valida los campos, controla `IsBusy` y llama a `ILoginService`.
4. `ScopedLoginService` crea un alcance por intento, obtiene `AuthenticationService`
   y libera el alcance al terminar, incluso ante errores.
5. La vista limpia los controles al terminar una consulta, restablece el modo oculto
   y devuelve el foco a la contraseña; si tiene éxito, establece `DialogResult`.
6. `App` recibe `AuthenticatedUser` y abre la ventana principal.

La contraseña no se almacena en una propiedad del ViewModel ni se enlaza mediante
`CommandParameter`. Se entrega como argumento temporal porque el servicio existente
requiere `string`; esto no supone un borrado garantizado de cadenas en memoria.
La copia del campo visible se elimina al ocultarla y ambos controles se limpian al
cerrar la ventana. Mientras se autentica no se permite editar ni cerrar.

Si faltan campos, se muestra el mensaje y se conservan sus valores, como antes.
Si la consulta rechaza el acceso, se conserva el usuario, se borra la contraseña y
se muestra el error general. No se identifica cuál credencial fue incorrecta.
No se introducen esperas artificiales para hacer visible el estado de carga.

## Navegación y sesión

`MainWindowViewModel` recibe el usuario autenticado, `IScreenFactory` e `IDialogService`.
Expone `SelectedScreen`, `CurrentViewModel`, el texto de sesión y los comandos.

- `NavigateCommand` comprueba el permiso en `CanExecute` y en la propia acción.
- Administración se oculta si el usuario no es Administrador.
- La fábrica entrega un ViewModel nuevo por navegación. No se conserva un caché de formularios.
- Los `DataTemplate` de `MainWindow.xaml` relacionan ViewModels con vistas.
- Un estilo común compara el destino del botón con la pantalla seleccionada para resaltarlo.
- `LogoutCommand` emite una solicitud; `App` limpia la sesión, cierra la ventana y
  abre un login nuevo. La suscripción se retira cuando se cierra la ventana principal.

El contrato de diálogos mantiene `MessageBox` fuera de los ViewModels.
Ocultar botones no sustituye la autorización de las operaciones de negocio futuras.

## Procedimiento para desarrollar un módulo

Trabajar en entregas completas y revisables: propuesta, implementación, comprobaciones
y revisión manual. No es necesario autorizar cada propiedad por separado si el alcance
de la entrega ya fue aprobado. Paquetes, base de datos, ramas y commits siguen
requiriendo su autorización correspondiente.

### 1. Definir la primera operación

Para pacientes, comenzar por listado y búsqueda. Definir campos, permisos, estados de
carga, lista vacía y errores antes de implementar edición o eliminación.

### 2. Crear el contrato y el servicio necesario

Por ejemplo, `Services/Patients/IPatientService.cs` y `PatientService.cs`. El servicio
consulta y aplica reglas de negocio; el ViewModel no utiliza EF Core directamente.
Reutilizar entidades y configuración existentes. No construir interfaces con operaciones
que el consumidor no necesita.

Los ViewModels se resuelven como transitorios desde la composición. Por tanto, no
inyectar directamente un `DbContext` o servicio scoped en ellos desde el proveedor raíz.
Usar una operación de servicio que cree y libere su alcance, como el adaptador del login,
o diseñar explícitamente una fábrica de contextos cuando ese módulo lo requiera.
No compartir un contexto entre operaciones concurrentes ni conservarlo toda la sesión.

### 3. Implementar el ViewModel

Agregar propiedades, lista observable, selección y comandos a `PatientsViewModel`.
Usar una sola fuente de verdad para el estado de ejecución de cada operación.
Manejar errores sin exponer datos privados. Evitar `.Wait()` y `.Result` en la interfaz.
Para formularios de edición, separar el borrador de la entidad del listado para permitir cancelar.

### 4. Enlazar la vista

Enlazar campos, selección, comandos y mensajes en XAML. Conservar en `.xaml.cs`
solo interacciones de controles. Los constructores y el diseñador no deben consultar
la base. Definir explícitamente cuándo se carga la pantalla y evitar cargas duplicadas.

### 5. Registrar la composición y la navegación

Pacientes ya está registrado. Para una pantalla nueva:

1. Crear su ViewModel y vista.
2. Registrar el ViewModel como transitorio en `AddPresentationServices`.
3. Agregar su identificador en `Screen` y su fábrica en el diccionario de composición.
4. Agregar el `DataTemplate` en los recursos de `MainWindow.xaml`.
5. Agregar el botón con `NavigationButtonStyle`, `Tag` y `CommandParameter` del nuevo destino.
6. Incorporar el permiso en la lógica de acceso y la visibilidad del menú si corresponde.

Los servicios deben entrar por constructor. No acceder al contenedor desde los ViewModels.

### 6. Comprobar la entrega

Probar el ViewModel con servicios simulados, compilar y comprobar la pantalla en
Visual Studio. Verificar lista vacía, éxito, errores, acciones repetidas y permisos.
Cambios de esquema, catálogos o reglas existentes se proponen por separado.

Extraer comportamiento común cuando exista duplicación real. Una clase base debe
respetar los contratos de sus derivados y no obligarlos a implementar acciones sin sentido.

## Verificaciones automáticas sin base de datos

Existe un ejecutable de comprobación sin paquetes de pruebas adicionales en
`tests/DentalControl.Verification`. No está añadido a la solución para conservar su
organización actual. No es un proyecto de Test Explorer.

Desde la raíz:

```powershell
dotnet build DentalControl.sln --no-restore --configuration Debug
dotnet restore tests/DentalControl.Verification/DentalControl.Verification.csproj
dotnet build tests/DentalControl.Verification/DentalControl.Verification.csproj --no-restore
dotnet tests/DentalControl.Verification/bin/Debug/net8.0-windows/DentalControl.Verification.dll
```

El último comando ejecuta únicamente las comprobaciones: no crea `App`, ventanas,
contextos ni conexiones SQLite. Comprueba estados del login, notificaciones, errores,
intentos simultáneos, navegación, permisos, instancias y solicitud de salida.
No sustituye las pruebas de integración del servicio real ni las de interfaz.

## Comprobación manual en Visual Studio 2022

Usar el entorno de datos de prueba acordado. El arranque existente puede migrar e
inicializar datos y el servicio real puede renovar un hash; copiar el repositorio
no cambia la ubicación de SQLite en `%LocalAppData%`.

1. Compilar la solución y revisar la ventana Salida.
2. Ejecutar con F5 y comparar con las capturas de referencia: colores, tamaños y textos.
3. Probar campos vacíos, Enter y contraseña incorrecta oculta y visible.
4. Después del rechazo, comprobar usuario conservado, contraseña vacía, modo oculto y foco.
5. Mostrar, ocultar y volver a mostrar una contraseña ficticia antes de enviarla: el texto debe conservarse correctamente.
6. Durante una consulta pendiente, comprobar que se bloquean controles y cierre.
7. Acceder como Administrador, recorrer las cinco pantallas y comprobar el resaltado.
8. Cerrar sesión, entrar como Doctor y comprobar que Administración no aparece.
9. Repetir el ciclo para descartar estado o suscripciones de la sesión anterior.
10. Cerrar la ventana principal y, en otra ejecución, el login; ambos deben terminar la aplicación.

Revisar Salida por errores de binding. El foco, la limpieza de controles reales, el
diseño, las transiciones de ventanas y el login contra SQLite quedan sujetos a esta
prueba manual; una compilación correcta no confirma esos comportamientos.
