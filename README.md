# QuickCareSim

**QuickCareSim** es un proyecto de simulación de atención de pacientes en una sala de urgencias, desarrollado como parte de la asignatura **Programación Paralela**.

Este sistema simula el flujo de pacientes según distintos niveles de urgencia, con múltiples médicos atendiendo en paralelo, utilizando tecnologías modernas y arquitectura escalable.

---

## 🛠️ Tecnologías Utilizadas

- **ASP.NET Core**
- **Razor Pages** (para la interfaz de usuario)
- **Arquitectura Onion (Onion Architecture)**
- **Programación Paralela con Task y ConcurrentQueue**

---

## 👥 Integrantes del Grupo

- Delio Rodríguez  
- Dayner García  
- Ydan Monegro  
- Raynier Zorrilla

---

## 🎯 Objetivo

Desarrollar una simulación eficiente que demuestre las ventajas del paralelismo en la atención médica de urgencias, permitiendo medir métricas como el tiempo promedio de atención, número de pacientes atendidos y eficiencia por médico.

---

## 📁 Estructura del Proyecto

El proyecto está dividido en capas siguiendo la arquitectura Onion:

- **Domain**: Entidades y enums centrales.
- **Application**: Lógica de negocio y servicios.
- **Infrastructure**: Implementaciones y acceso a datos.
- **UI**: Interfaz web desarrollada con Razor.

---


## 🚀 Cómo ejecutar el proyecto

1. Clona el repositorio.
2. Configura la cadena de conexión en `appsettings.json`.
3. Ejecuta el proyecto desde Visual Studio o con `dotnet run`.

---

## 📚 Créditos

Proyecto desarrollado para fines académicos, demostrando buenas prácticas de arquitectura, concurrencia y simulación en C#.

---
