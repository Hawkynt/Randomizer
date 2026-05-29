<?php
// PHP 7.0+ ships with two cryptographically-secure functions:
//   random_bytes(n)   - raw bytes from the OS entropy source
//   random_int(a, b)  - uniform int in [a, b] from the same source
// Both throw an Exception if the platform has no secure RNG available.

try {
    $bytes = random_bytes(8); // 64 bits
    printf("Random 64-bit number: %s\n", bin2hex($bytes));
} catch (Exception $e) {
    fwrite(STDERR, "Failed to generate random bytes: " . $e->getMessage() . "\n");
    exit(1);
}
