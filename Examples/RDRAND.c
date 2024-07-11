#include <immintrin.h>
#include <stdio.h>

int main() {
  unsigned long long random_number;

  if (_rdrand64_step(&random_number)) {
    printf("Random 64-bit number: %016llx\n", random_number);
  }
  else {
    printf("Failed to generate random number\n");
  }

  return 0;
}