# Julia: stdlib `Random` is built on Xoshiro256++ (seeded from RandomDevice
# at startup) — fast and good quality but NOT cryptographically secure. For
# crypto use `RandomDevice` directly (calls /dev/urandom or equivalent).

using Random

# Fast Xoshiro256++ — recommended default for Monte Carlo, simulation, ML:
n = rand(UInt64)
println("Random 64-bit number (Xoshiro256++):  ", string(n, base = 16, pad = 16))

# Cryptographically secure via the OS entropy source:
rd = RandomDevice()
m = rand(rd, UInt64)
println("Random 64-bit number (OS entropy):    ", string(m, base = 16, pad = 16))
