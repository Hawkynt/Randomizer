# R: the standard `runif`, `sample`, `set.seed` family is built on Mersenne
# Twister (NOT cryptographically secure — designed for statistics and
# simulation). For crypto-grade randomness use the `openssl` package:
#   openssl::rand_bytes(8)
#
# This example shows both — first the statistical default, then the
# crypto-grade alternative.

set.seed(131)  # deterministic seed for reproducible Monte Carlo runs
n <- floor(runif(1) * 2^64)
cat(sprintf("Random 64-bit number (Mersenne Twister): %s\n",
            format(n, scientific = FALSE)))

# Crypto-grade (requires the 'openssl' package: install.packages("openssl"))
if (requireNamespace("openssl", quietly = TRUE)) {
  bytes <- openssl::rand_bytes(8)
  hex <- paste(format(bytes), collapse = "")
  cat(sprintf("Random 64-bit number (OpenSSL):          %s\n", hex))
}
