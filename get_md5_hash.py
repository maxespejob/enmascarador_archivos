import os
import hashlib
 
# Carpeta donde están los archivos
DIRECTORY = "./Downloads/publish/"
 # 20260203_111838.ctf  ->  329d6b104f1a6aac1ae561e57081c437
 # 20260203_111838_masked.ctf  ->  388c5fcbc58609ca348e6f60a73155cf
# Tamaño del bloque de lectura (8KB)
BUFFER_SIZE = 8192
 
 
def calculate_md5(file_path):
    """Calcula el MD5 de un archivo leyendo en bloques"""
    md5 = hashlib.md5()
 
    with open(file_path, "rb") as f:
        while chunk := f.read(BUFFER_SIZE):
            md5.update(chunk)
 
    return md5.hexdigest()
 
 
def main():
 
    if not os.path.exists(DIRECTORY):
        print(f"La carpeta {DIRECTORY} no existe")
        return
 
    print(f"Calculando hashes MD5 en: {DIRECTORY}\n")
 
    for root, dirs, files in os.walk(DIRECTORY):
 
        for file in files:
 
            file_path = os.path.join(root, file)
 
            try:
                file_hash = calculate_md5(file_path)
 
                print(f"{file}  ->  {file_hash}")
 
            except Exception as e:
                print(f"Error procesando {file}: {e}")
 
 
if __name__ == "__main__":
    main()