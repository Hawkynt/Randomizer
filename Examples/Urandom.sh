#!/usr/bin/env bash
# Read 8 bytes (64 bits) from the kernel's blocking-free cryptographic RNG.
#
# /dev/urandom on Linux/BSD/macOS draws from the same CSPRNG kernel pool as
# /dev/random, but never blocks waiting for entropy estimation — once seeded,
# it stays seeded for the lifetime of the kernel.
#
# Note: Bash's built-in $RANDOM is only 15 bits and is NOT cryptographically
# secure; use /dev/urandom or `openssl rand -hex 8` for anything that matters.

set -euo pipefail

random_hex=$(head -c 8 /dev/urandom | od -A n -t x1 | tr -d ' \n')
echo "Random 64-bit number: ${random_hex}"
