import ctypes
import os

def get_random_bytes(n):
  buf = (ctypes.c_ubyte * n)()
  result = ctypes.cdll.LoadLibrary(ctypes.util.find_library("Security")).SecRandomCopyBytes(None, n, buf)
  if result != 0:
    raise ValueError("Failed to generate random bytes")
  return bytes(buf)

random_bytes = get_random_bytes(8)  # Generate 64-bit random number
print("Random 64-bit number: ", random_bytes.hex())