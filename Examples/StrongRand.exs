# Elixir (and Erlang's :crypto module): :crypto.strong_rand_bytes is the
# canonical cryptographic source, wrapping OpenSSL's RAND_bytes. For fast
# non-cryptographic use, :rand.uniform draws from Xoshiro256++ seeded from
# OS entropy at process start.
#
# Run: elixir StrongRand.exs

# Cryptographic:
<<n::unsigned-integer-size(64)>> = :crypto.strong_rand_bytes(8)
IO.puts("Random 64-bit number (cryptographic):     #{Integer.to_string(n, 16) |> String.pad_leading(16, "0")}")

# Non-cryptographic (Xoshiro256++):
m = :rand.uniform(Bitwise.bsl(1, 64)) - 1
IO.puts("Random 64-bit number (non-cryptographic): #{Integer.to_string(m, 16) |> String.pad_leading(16, "0")}")
