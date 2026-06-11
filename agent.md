# Agent Context & Solution Architecture 🤖

Este archivo contiene el contexto técnico estricto de **DocuMind RAG Core**. Cualquier asistente de IA que trabaje en este repositorio debe leer y respetar las siguientes directrices de diseño para mantener la consistencia del código.

---

## 🏗️ Reglas de Arquitectura (Clean Architecture)
1. **DocuMind.Domain**: Cero dependencias externas. Contiene las entidades puras. La propiedad `Embedding` usa el tipo `Pgvector.Vector`.
2. **DocuMind.Application**: Contiene la lógica de negocio fragmentada mediante comandos y consultas (CQRS) con **MediatR**. Las interfaces de servicios e infraestructura (`IEmbeddingService`, `IChatService`, `IDocumentRepository`) se definen aquí.
3. **DocuMind.Infrastructure**: Implementa el acceso a datos con EF Core y la comunicación HTTP externa. No expone lógica de negocio.
4. **DocuMind.WebApi**: Punto de entrada, controladores REST y lectura de configuraciones.

---

## 🔧 Configuración e Inyección de Dependencias
* **Fábrica Dinámica:** Los servicios de IA (`IEmbeddingService` y `IChatService`) se registran en `DependencyInjection.cs` de la infraestructura mediante una fábrica que lee las URLs y modelos desde el `appsettings.json`.
* **URLs Dinámicas:** Está prohibido quemar URLs o nombres de modelos en las clases. Todo se construye en la inyección combinando `BaseUrl`, `EmbeddingModel` o `ChatModel` con la `ApiKey`.
* **Prompting:** El System Prompt del RAG es dinámico. Se almacena en `PromptSettings:SystemPrompt` dentro del `appsettings.json` y utiliza los marcadores `{context}` y `{query}` para la sustitución de texto en el Query Handler.

---

## 📊 Base de Datos Vectorial
* Motor: PostgreSQL con la extensión `pgvector`.
* Mapeo: Manejado por `Pgvector.EntityFrameworkCore`. La columna de embedding está configurada explícitamente como `vector(768)` en el `DbContext` para alinearse con el modelo `text-embedding-004`.