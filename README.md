# Sistema de Gestión Escolar - Instituto "San Martín" 🏫

Proyecto desarrollado para la asignatura **Ingeniería de Software (2026)**. 
Este sistema permite gestionar integralmente la solicitud, validación documental y actualización de estados de inscripción de alumnos, controlando automáticamente el cupo de vacantes por curso y manteniendo un padrón consolidado.

## 👥 Equipo de Desarrollo (Grupo 1)
* Achenapure Santiago
* Barbeito Matías
* Di Bartolo Daniel
* Pajon Priscila
* Romero Gastón

## 🛠️ Stack Tecnológico
* **Backend:** C# .NET 8 (Web API)
* **Frontend:** HTML5, CSS3, Vanilla JavaScript (servido estáticamente desde `wwwroot`)
* **Base de Datos:** MySQL 8.0
* **Despliegue:** 100% Dockerizado (Contenedor con API + Motor MySQL integrado)
* **ORM:** Entity Framework Core (Pomelo MySQL)

## 🚀 Cómo ejecutar el proyecto en entorno local

El proyecto está diseñado con una arquitectura auto-contenida. No es necesario tener MySQL instalado físicamente en la computadora ni ejecutar scripts manuales, ya que el sistema crea la base de datos y los datos semilla automáticamente al iniciar.

### Opción A: Desde Visual Studio 2022 (Recomendado)
1. Clonar este repositorio:
   ```bash
   git clone [https://github.com/santiAchee/ProyectoEscolar.git](https://github.com/santiAchee/ProyectoEscolar.git)
