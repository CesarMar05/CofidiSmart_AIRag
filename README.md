# AT2Soft.RAGEngine

**AT2Soft.RAGEngine** es una API desarrollada en .NET que permite enriquecer consultas de Lenguaje Natural con contexto relevante extraído desde documentos empresariales (PDFs, XMLs, Excel, etc.), gracias a una integración con LangChain y modelos de lenguaje como Ollama.

Este motor permite construir soluciones de consulta inteligentes (tipo ChatGPT) sobre bases de conocimiento privadas, manteniendo la información organizada, consultable y segura.

---

## 🧠 ¿Qué es lo que hace?

`AT2Soft.RAGEngine` implementa el patrón RAG (Retrieval-Augmented Generation), el cual consta de dos fases:

1. **Indexación del conocimiento**  
   Extrae y divide el contenido de documentos en fragmentos (chunks), genera embeddings con el modelo `nomic-embed-text` de Ollama, y los almacena en una base vectorial (por ejemplo, Chroma o Qdrant).

2. **Consulta enriquecida**  
   Dado un `prompt` del usuario, el sistema busca los fragmentos más relevantes en la base vectorial, los concatena como contexto, y realiza una consulta a un modelo LLM (por ejemplo, `llama3` u otro local u hospedado) para generar una respuesta fundamentada.

---

## 🔧 Funcionalidades principales

- 📄 Soporte para múltiples formatos: PDF, Excel, XML.
- 🧠 Generación de embeddings vía Ollama.
- 🗃 Almacenamiento vectorial usando Chroma o Qdrant.
- 💬 Consulta semántica con inyección de contexto.
- 🧩 Modular y extensible para múltiples fuentes o modelos.
- 🔐 Ideal para entornos privados o desconectados de internet.

---

## 🚀 Tecnologías utilizadas

- .NET 8 / ASP.NET Core
- LangChain (Python)
- Ollama (Embeddings + LLM)
- Chroma / Qdrant (vector database)
- Docker

---

## 🏗 Arquitectura básica

[Usuario] —> [API .NET] —> [LangChain] —> [Vector DB]
|                 ↑
↓                 |
[Modelo Ollama] <— [Documentos indexados]

---

## 📁 Estructura del proyecto

AT2Soft.RAGEngine/
│
├── src/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   └── LangChainInterop/
│
├── data/                  # Documentos cargados
├── embeddings/            # Base vectorial (ej. Chroma, Qdrant)
└── README.md

---

## 📜 Historial de versiones

| Versión | Fecha       | Descripción breve                     |
|---------|-------------|----------------------------------------|
| 0.1.0   | 2025-07-24  | Versión inicial: estructura básica, carga de documentos, generación de embeddings y consulta a modelo vía LangChain. |
| ...     | ...         | ...                                    |

---

## 📌 Próximos pasos

- [ ] Agregar autenticación a la API
- [ ] Habilitar múltiples colecciones de conocimiento
- [ ] Panel de administración web (opcional)
- [ ] Integración con Azure Blob / AWS S3 para almacenamiento

---

## 🧾 Licencia

Este proyecto es propiedad de **AT2Soft**. El uso está limitado a propósitos internos o bajo acuerdo comercial.

---

## ✉ Contacto

Para soporte o colaboración:
**atoledo@at2soft.com**

---