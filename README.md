# 🚀 Proyecto Blazor - Patrón MVC

Este proyecto implementa un **patrón MVC** (Modelo - Vista - Controlador) utilizando **Blazor**.  
El objetivo es mantener una arquitectura **ordenada y escalable**, separando responsabilidades de manera clara.

---

## 📂 Estructura de Carpetas

```bash
📦 ProyectoBlazorMVC/
│
├── 📁 Models/            # Representación de datos y reglas de negocio
│   ├── CitaModel.cs
│   ├── ClienteModel.cs
│   ├── CompraModel.cs
│   ├── EmpleadoModel.cs
│   ├── InventarioModel.cs
│   ├── ProductoModel.cs
│   ├── ProveedorModel.cs
│   ├── TurnoModel.cs
│   └── VentaModel.cs
│
├── 📁 Controllers/       # Controladores: manejan la lógica de la aplicación
│   ├── CitaController.cs
│   ├── ClienteController.cs
│   ├── CompraController.cs
│   ├── EmpleadoController.cs
│   ├── InventarioController.cs
│   ├── ProductoController.cs
│   ├── ProveedorController.cs
│   ├── TurnoController.cs
│   └── VentaController.cs
│
├── 📁 Views/             # Vistas (Razor Components) - Interfaz de usuario
│   ├── Empleado/
│   │   ├── Lista.razor
│   │   └── Detalle.razor
│   ├── Cita/
│   │   ├── Calendario.razor
│   │   └── Nueva.razor
│   └── Turno/
│       ├── Lista.razor
│       └── Asignar.razor
│
├── 📁 Routes/            # Definición de rutas para las vistas y controladores
│   ├── EmpleadoRoutes.cs
│   ├── CitaRoutes.cs
│   └── TurnoRoutes.cs
│
├── 📁 wwwroot/            # Archivos estáticos (CSS, JS, imágenes)
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── script.js
│   ├── lib/
│   └── favicon.ico
│
├── App.razor            # Archivo principal que configura la aplicación Blazor
├── Program.cs           # Punto de entrada de la app
└── README.md

## ⚙️ Tecnologías Usadas

- Blazor Server / WebAssembly (según el caso)
- C# para lógica de negocio y controladores
- Razor Components para las vistas
- .NET 8+
- Bootstrap / Tailwind (opcional para estilos)
- Git para control de versiones

## 🛠️ Instalación y Ejecución

1. **Clona el repositorio:**

```bash
git clone https://github.com/tuusuario/ProyectoBlazorMVC.git
cd ProyectoBlazorMVC```


Abre en el navegador:

https://localhost:4200


🧩 Patrón MVC Aplicado

- Modelos: Definen entidades y reglas de negocio (Empleado, Cita, Turno).
- Controladores: Gestionan la lógica de cada módulo y comunican modelos con vistas.
- Vistas: Interfaz de usuario, renderizada en componentes .razor.
- Routes: Centraliza las rutas de navegación para mantener código limpio.
- Public: Recursos estáticos que no requieren compilación.

📌 Notas Importantes
- El archivo App.razor es la entrada de las vistas, pero las páginas se cargan desde la carpeta Views usando rutas.
- Las rutas están separadas en la carpeta routes para mejorar la organización y evitar sobrecargar el App.razor.
- La carpeta public expone recursos estáticos como imágenes, estilos y scripts.
