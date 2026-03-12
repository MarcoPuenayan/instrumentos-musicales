# Sistema de Registro de Instrumentos Musicales

Sistema de escritorio desarrollado en .NET 8 con Windows Forms para la gestion integral de instrumentos musicales, repuestos, asignaciones y personas dentro de una organizacion. Permite llevar un inventario actualizado, controlar la asignacion de instrumentos y repuestos a personas, y administrar los usuarios del sistema con control de roles.

---

## Caracteristicas principales

- **Gestion de Personas**: Registro de personal con cedula, datos de contacto, departamento y estado activo/inactivo.
- **Inventario de Instrumentos**: Control completo de instrumentos (codigo, tipo, marca, modelo, numero de serie, valor de adquisicion, estado).
- **Asignacion de Instrumentos**: Asignacion y devolucion de instrumentos a personas, con registro de fecha, estado y observaciones.
- **Gestion de Repuestos**: Inventario de repuestos con control de stock, stock minimo, categoria y costo. Los repuestos pueden vincularse opcionalmente a un instrumento.
- **Asignacion de Repuestos**: Entrega de repuestos a personas con registro de cantidad, motivo y usuario responsable.
- **Gestion de Usuarios**: Administracion de cuentas de usuario con roles diferenciados (Administrador / Operador).
- **Autenticacion segura**: Login con contrasenas encriptadas mediante BCrypt. Solo el Administrador accede al modulo de gestion de usuarios.
- **Base de datos local**: SQLite embebido, creado automaticamente al primer inicio. No requiere instalacion de servidor.
- **Interfaz MDI**: Ventana principal con multiples formularios hijos, barra de menu y barra de estado con usuario activo y fecha.

---

## Tecnologias utilizadas

| Componente        | Tecnologia / Version                       |
|-------------------|--------------------------------------------|
| Framework         | .NET 8.0 (net8.0-windows)                  |
| Interfaz grafica  | Windows Forms (WinForms)                   |
| ORM               | Entity Framework Core 8.0.13               |
| Base de datos     | SQLite (Microsoft.EntityFrameworkCore.Sqlite 8.0.13) |
| Encriptacion      | BCrypt.Net-Next 4.0.3                      |
| Lenguaje          | C# 12                                      |

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8) instalado (para compilar desde codigo fuente).
- Windows 10 / 11 (64 bits recomendado).
- No se requiere instalar ningun motor de base de datos externo.

---

## Como ejecutar

### Opcion 1: Desde el codigo fuente

```bash
# Clonar o descomprimir el proyecto
cd registroinstrumentos

# Restaurar dependencias y compilar
dotnet build RegistroInstrumentos/RegistroInstrumentos.csproj

# Ejecutar la aplicacion
dotnet run --project RegistroInstrumentos/RegistroInstrumentos.csproj
```

### Opcion 2: Ejecutar el binario compilado

Si ya cuenta con el archivo `.exe` publicado, simplemente ejecute:

```
RegistroInstrumentos.exe
```

> La base de datos `registro_instrumentos.db` se creara automaticamente en la misma carpeta donde se encuentra el ejecutable al primer inicio.

---

## Credenciales por defecto

Al iniciar la aplicacion por primera vez, se crea automaticamente el siguiente usuario administrador:

| Campo          | Valor          |
|----------------|----------------|
| Usuario        | `admin`        |
| Contrasena     | `Admin123!`    |
| Nombre completo| Administrador del Sistema |
| Rol            | Administrador  |

> Se recomienda cambiar la contrasena del administrador despues del primer inicio de sesion.

---

## Estructura del proyecto

```
RegistroInstrumentos/
|
+-- Models/                        # Entidades del dominio
|   +-- Usuario.cs                 # Usuarios del sistema (rol Administrador | Operador)
|   +-- Persona.cs                 # Personas/personal con cedula y departamento
|   +-- Instrumento.cs             # Instrumentos musicales con estado y valor
|   +-- Repuesto.cs                # Repuestos con stock y costo
|   +-- AsignacionInstrumento.cs   # Relacion Instrumento <-> Persona
|   +-- AsignacionRepuesto.cs      # Relacion Repuesto <-> Persona con cantidad
|
+-- Data/
|   +-- AppDbContext.cs            # Contexto EF Core, configuracion y seed inicial
|
+-- Repositories/                  # Acceso a datos por entidad
|   +-- UsuarioRepository.cs
|   +-- PersonaRepository.cs
|   +-- InstrumentoRepository.cs
|   +-- RepuestoRepository.cs
|   +-- AsignacionInstrumentoRepository.cs
|   +-- AsignacionRepuestoRepository.cs
|
+-- Services/
|   +-- AuthService.cs             # Logica de autenticacion, sesion y roles
|
+-- Forms/                         # Formularios de la interfaz grafica
|   +-- LoginForm.cs               # Pantalla de inicio de sesion
|   +-- MainForm.cs                # Ventana principal MDI con menu de navegacion
|   +-- PersonaForm.cs             # Gestion de personas
|   +-- InstrumentoForm.cs         # Inventario de instrumentos
|   +-- AsignacionInstrumentoForm.cs # Asignar/devolver instrumentos
|   +-- RepuestoForm.cs            # Gestion de repuestos
|   +-- AsignacionRepuestoForm.cs  # Asignar repuestos a personas
|   +-- UsuarioForm.cs             # Gestion de usuarios (solo Administrador)
|
+-- Helpers/
|   +-- PasswordHelper.cs          # Utilidad para hash y verificacion BCrypt
|
+-- Program.cs                     # Punto de entrada, inicializacion de DB y login
+-- RegistroInstrumentos.csproj    # Definicion del proyecto y dependencias
```

---

## Base de datos

- **Motor**: SQLite (archivo local, no requiere servidor).
- **Nombre del archivo**: `registro_instrumentos.db`
- **Ubicacion**: Se genera automaticamente en la misma carpeta del ejecutable (`AppDomain.CurrentDomain.BaseDirectory`).
- **Creacion automatica**: La base de datos y todas las tablas se crean al primer inicio mediante `Database.EnsureCreated()`.
- **Datos iniciales (Seed)**: Se inserta el usuario administrador por defecto si no existe ningun usuario en la base de datos.

### Relaciones principales

```
Persona  1 ----< AsignacionInstrumento >---- 1  Instrumento
Persona  1 ----< AsignacionRepuesto    >---- 1  Repuesto
Instrumento 1 ----< Repuesto  (relacion opcional, repuesto puede no estar ligado a un instrumento)
```

### Estados de un Instrumento

| Estado          | Descripcion                              |
|-----------------|------------------------------------------|
| `Disponible`    | Libre para ser asignado                  |
| `Asignado`      | Actualmente asignado a una persona       |
| `En Reparacion` | Fuera de servicio temporalmente          |
| `Dado de Baja`  | Retirado del inventario definitivamente  |

---

## Modulo de autenticacion

- El inicio de sesion se gestiona a traves de `AuthService`.
- Las contrasenas se almacenan encriptadas con **BCrypt** (algoritmo de hash adaptativo).
- Se mantiene la sesion activa en `AuthService.UsuarioActual` durante la ejecucion.
- El menu **Administracion** (Gestion de Usuarios) solo es visible para usuarios con rol `Administrador`.
- Al cerrar sesion, la aplicacion se reinicia y vuelve a mostrar el formulario de login.

---

## Menu de navegacion

| Menu principal   | Opciones disponibles                    | Acceso          |
|------------------|-----------------------------------------|-----------------|
| Personas         | Registrar / Gestionar Personas          | Todos            |
| Instrumentos     | Inventario de Instrumentos              | Todos            |
|                  | Asignar Instrumento                     | Todos            |
| Repuestos        | Registro de Repuestos                   | Todos            |
|                  | Asignar Repuesto a Persona              | Todos            |
| Administracion   | Gestion de Usuarios                     | Solo Administrador |
| Cerrar Sesion    | Cierra la sesion y vuelve al login      | Todos            |

---

## Capturas de pantalla

> _Proximamente: capturas de la pantalla de login, formulario principal, inventario de instrumentos y modulo de asignaciones._

---

## Licencia

Este proyecto se distribuye bajo los terminos de la licencia **MIT**.

```
MIT License

Copyright (c) 2026

Se concede permiso, de forma gratuita, a cualquier persona que obtenga una copia
de este software y los archivos de documentacion asociados, para utilizar el software
sin restricciones, incluyendo sin limitacion los derechos de usar, copiar, modificar,
fusionar, publicar, distribuir, sublicenciar y/o vender copias del software, con sujecion
a las siguientes condiciones:

El aviso de copyright anterior y este aviso de permiso se incluiran en todas las copias
o partes sustanciales del software.

EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTIA DE NINGUN TIPO.
```
