import torch
torch.set_default_device("cpu")
import os
os.environ["PYTORCH_ENABLE_MPS_FALLBACK"] = "1"

from datasets import load_dataset
from transformers import AutoTokenizer, AutoModelForCausalLM, TrainingArguments, Trainer, DataCollatorForLanguageModeling
from peft import get_peft_model, LoraConfig, TaskType
from huggingface_hub import login
from transformers import AutoModelForCausalLM

# (Solo si es necesario) inicia sesión con tu token de Hugging Face
# login("hf_...")

model_name = "mistralai/Mistral-7B-Instruct-v0.1"
dataset_path = "data/data.jsonl"

# Cargar tokenizer y modelo base
tokenizer = AutoTokenizer.from_pretrained(model_name, use_fast=True)
tokenizer.pad_token = tokenizer.eos_token

device = torch.device("cpu")
model = AutoModelForCausalLM.from_pretrained(
    model_name,
    torch_dtype=torch.float32,
    low_cpu_mem_usage=False
).to(device)

# Configurar LoRA
peft_config = LoraConfig(
    r=8,
    lora_alpha=16,
    task_type=TaskType.CAUSAL_LM,
    lora_dropout=0.05,
    bias="none"
)
model = get_peft_model(model, peft_config)

# Cargar dataset
dataset = load_dataset("json", data_files=dataset_path, split="train")

# Tokenizar ejemplos
def tokenize(example):
    prompt = f"""### Pregunta:
{example['input']}

### Respuesta:
{example['output']}
"""
    return tokenizer(prompt, truncation=True, padding="max_length", max_length=512)

tokenized_dataset = dataset.map(tokenize)

# Argumentos de entrenamiento
training_args = TrainingArguments(
    output_dir="./lora_model",
    num_train_epochs=5,
    per_device_train_batch_size=1,
    logging_steps=1,
    save_strategy="epoch",
    fp16=False,  # <- no usar fp16 en Mac (solo si tienes GPU NVIDIA)
    report_to="none"
)

trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=tokenized_dataset,
    tokenizer=tokenizer,
    data_collator=DataCollatorForLanguageModeling(tokenizer, mlm=False)
)

trainer.train()