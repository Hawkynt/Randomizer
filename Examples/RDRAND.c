#include <immintrin.h>
#include <stdio.h>

/* Per Intel's "Digital Random Number Generator" software-implementation guide,
   RDRAND can fail when the on-chip entropy reservoir is briefly depleted; the
   recommended pattern is to retry up to 10 times before giving up. */
int main() {
  unsigned long long random_number;
  int success = 0;
  for (int i = 0; i < 10 && !success; ++i)
    success = _rdrand64_step(&random_number);

  if (success) {
    printf("Random 64-bit number: %016llx\n", random_number);
    return 0;
  }

  fprintf(stderr, "Failed to generate random number after 10 retries\n");
  return 1;
}