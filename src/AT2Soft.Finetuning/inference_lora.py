import torch
from transformers import AutoTokenizer, AutoModelForCausalLM, GenerationConfig
from peft import PeftModel, PeftConfig

# Configuraciones
base_model = "mistralai/Mistral-7B-Instruct-v0.1"
lora_model = "./lora_model/checkpoint-10"  # puedes cambiar por "./lora_model_final" si consolidaste

# Cargar tokenizer
tokenizer = AutoTokenizer.from_pretrained(base_model, use_fast=True)
tokenizer.pad_token = tokenizer.eos_token

# Cargar modelo base y aplicar LoRA
print("Cargando modelo base...")
model = AutoModelForCausalLM.from_pretrained(
    base_model,
    torch_dtype=torch.float16 if torch.backends.mps.is_available() else torch.float32,
    device_map="auto"  # ajusta automáticamente a MPS o CPU
)

print(f"Cargando LoRA desde {lora_model}...")
model = PeftModel.from_pretrained(model, lora_model)

# Fusionar LoRA (opcional)
# model = model.merge_and_unload()
# model.save_pretrained("./merged_model")

# Prompt de ejemplo
prompt = """### Pregunta:
¿Cuál es la capital de Francia?

### Respuesta:"""

inputs = tokenizer(prompt, return_tensors="pt").to(model.device)

print("Generando respuesta...")
with torch.no_grad():
    outputs = model.generate(
        **inputs,
        max_new_tokens=100,
        do_sample=True,
        top_p=0.95,
        temperature=0.7,
        pad_token_id=tokenizer.eos_token_id
    )

print("\n🧠 Respuesta generada:\n")
print(tokenizer.decode(outputs[0], skip_special_tokens=True))