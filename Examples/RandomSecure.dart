// Dart: `Random.secure()` uses the platform's CSPRNG (BCryptGenRandom on
// Windows, /dev/urandom on POSIX, the web Crypto API in Flutter Web). For
// fast non-cryptographic use, the default `Random()` constructor uses a
// PRNG seeded from the current time.

import 'dart:math';

void main() {
  final rng = Random.secure();

  // Compose a 64-bit value from two 32-bit calls — Random.nextInt only
  // accepts values up to 2^32.
  final hi = rng.nextInt(1 << 32);
  final lo = rng.nextInt(1 << 32);
  final n = (BigInt.from(hi) << 32) | BigInt.from(lo);

  print('Random 64-bit number: ${n.toRadixString(16).padLeft(16, '0')}');
}
