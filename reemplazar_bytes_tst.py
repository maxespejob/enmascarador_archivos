def reemplazar_bytes(ruta, offset, reemplazo):
    """
    Reemplaza bytes en un archivo binario.
 
    :param ruta: ruta al archivo binario
    :param offset: posición inicial (entero, base 10)
    :param reemplazo: lista de bytes en hex (ej: [0xF1, 0xF4, 0xF8])
    """
    with open(ruta, "rb+") as f:
        f.seek(offset)
        f.write(bytearray(reemplazo))
 
# ejemplo de uso
archivo = "Masking App/Files/Input/OMC_20260203_104358_0001"   # pon aquí tu archivos
# posicion = 0x1931F9B6      # offset (ejemplo en hexadecimal del dump que mostraste)
# nuevo_valor = [0xF0]
 
posicion = 0x71      # offset (ejemplo en hexadecimal del dump que mostraste)
nuevo_valor = [0xF1,0xF1,0xF1,0xF1,0xF1,0xF1,0xF1]
 
reemplazar_bytes(archivo, posicion, nuevo_valor)
print("Reemplazo completado ✅")
 