# DocuMind RAG Core 🧠📄

DocuMind es un motor de búsqueda semántica y de respuestas automatizadas basado en la arquitectura **RAG (Retrieval-Augmented Generation)**. Permite indexar documentos privados, fragmentarlos de manera inteligente, calcular sus huellas matemáticas (*embeddings*) y almacenarlos en una base de datos vectorial para responder preguntas de auditoría y cumplimiento con total precisión y cero alucinaciones.

Diseñado bajo los principios de **Clean Architecture**, **Domain-Driven Design (DDD)** y patrones **CQRS** con .NET 9/10.

---

## 🛠️ Arquitectura del Sistema

El proyecto está estructurado en capas desacopladas para garantizar la mantenibilidad y permitir el intercambio de componentes de IA o persistencia sin afectar las reglas de negocio:

* **`DocuMind.Domain`**: Contiene las entidades principales del negocio (como `DocumentChunk`) y tipos estructurados libres de dependencias externas.
* **`DocuMind.Application`**: Define los casos de uso del sistema estructurados mediante CQRS (Comandos y Consultas) utilizando **MediatR**. Maneja la lógica de fragmentación de texto (*Text Splitting*).
* **`DocuMind.Infrastructure`**: Implementa el acceso a datos mediante **Entity Framework Core** y conecta de manera dinámica con servicios de Inteligencia Artificial (Google Gemini o cualquier proveedor mediante API REST).
* **`DocuMind.WebApi`**: Punto de entrada de la aplicación que expone los endpoints RESTful parametrizados mediante `appsettings.json`.

---

## ⚙️ Tecnologías & Herramientas

* **Backend:** .NET 9 / .NET 10 (C#)
* **Base de Datos Vectorial:** PostgreSQL + Extensión `pgvector` montado sobre **Docker**.
* **Modelos de IA Integrados:** * *Traducción Matemática (Embeddings):* Google `text-embedding-004` (768 dimensiones).
    * *Generación de Respuestas (Chat):* Google `gemini-2.5-flash` (u otros modelos configurables).
* **Patrones de Diseño:** CQRS, Factory Pattern, Dependency Injection, Repository Pattern.

---

## 🚀 Guía de Configuración y Uso

### 1. Requisitos Previos
* SDK de .NET actual instalado.
* Docker Desktop o Rancher Desktop corriendo.

### 2. Levantar la Base de Datos Vectorial
Ejecuta el contenedor de PostgreSQL con soporte para vectores:
```bash
docker run --name documind-vector-db -e POSTGRES_USER=documind_user -e POSTGRES_PASSWORD=DocuMindSecurePass2026! -e POSTGRES_DB=documind_rag_db -p 5432:5432 -d pgvector/pgvector:pg16