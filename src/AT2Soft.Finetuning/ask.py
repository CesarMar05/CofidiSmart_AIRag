import requests

def ask_model(question: str, context: str = ""):
    prompt = f"""Responde la siguiente pregunta utilizando el contexto proporcionado.

Contexto:
{context}

Pregunta:
{question}
"""

    response = requests.post(
        "http://localhost:11434/api/generate",
        json={
            "model": "codellama:7b-instruct",
            "prompt": prompt,
            "stream": False
        }
    )

    if response.status_code != 200:
        raise Exception("Error consultando el modelo")

    data = response.json()
    print("\n📘 Respuesta del modelo:")
    print(data["response"])

# Puedes cambiar el contexto por algo real leído de archivo o RAG
context = """
COFIDI es una solución desarrollada por AT2Soft que permite la generación, validación y gestión de facturas electrónicas de acuerdo con las normas del SAT en México.
"""

ask_model("¿Qué es COFIDI?", context)
