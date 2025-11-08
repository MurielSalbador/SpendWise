# SpendWise 💰

**SpendWise** es una aplicación de gestión financiera personal desarrollada en **.NET 8**, diseñada para ayudarte a controlar tus ingresos, gastos y objetivos de ahorro de forma simple y eficiente.

---

## 🧩 Tecnologías utilizadas

<p align="center">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" width="80" height="80" alt="C#" title="C#" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="80" height="80" alt=".NET" title=".NET Core" />
  <img src="https://www.vectorlogo.zone/logos/mysql/mysql-icon.svg" width="80" height="80" alt="SQLite" title="MySQL" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/swagger/swagger-original.svg" width="80" height="80" alt="Swagger" title="Swagger" />
</p>

- **.NET 8.0** – Framework principal del backend
- **ASP.NET Core** – Framework web para la creación de APIs REST  
- **Entity Framework Core** – ORM para el manejo de datos  
- **SQLite** – Base de datos ligera y embebida  
- **Swagger** – Documentación y prueba de endpoints  
- **Clean Architecture** – Organización por capas para mantener un código limpio, escalable y mantenible

---

## 👥 Integrantes:
* Baptista Carvalho, Gabriela
* Salbador, Muriel
* Ríos, Elena

---

# Comandos para Migración

## Add migration

Actualiza database (en raíz del proyecto):

``` bash
  dotnet ef database update --context ApplicationDbContext --startup-project src/Web --project src/Infrastructure
```

Crear un nuevo cambio en database:

``` bash
  dotnet ef migrations add [nombredemigracion] --context ApplicationDbContext --startup-project src/Web --project src/Infrastructure -o Data/Migrations
```
