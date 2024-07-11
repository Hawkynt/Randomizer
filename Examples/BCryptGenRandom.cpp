#include <windows.h>
#include <bcrypt.h>
#include <iostream>

#pragma comment(lib, "bcrypt.lib")

int main() {
  UCHAR random_number[8]; // Generate 64-bit random number
  NTSTATUS status = BCryptGenRandom(
    NULL,
    random_number,
    sizeof(random_number),
    BCRYPT_USE_SYSTEM_PREFERRED_RNG
  );

  if (status == STATUS_SUCCESS) {
    std::cout << "Random 64-bit number: ";
    for (int i = 0; i < sizeof(random_number); i++) {
      printf("%02x", random_number[i]);
    }
    std::cout << std::endl;
  }
  else {
    std::cerr << "Failed to generate random number" << std::endl;
  }

  return 0;
}