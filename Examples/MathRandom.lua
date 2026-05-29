-- Lua 5.3+: math.random returns integers up to 2^53 (the IEEE-754 double
-- mantissa). For 64-bit values we concatenate two calls. The underlying
-- generator is platform-dependent (xoshiro256** in Lua 5.4) and is NOT
-- cryptographically secure — seed it deterministically for reproducible
-- simulations.
--
-- For crypto-grade randomness, the LuaCrypto module (or LuaJIT FFI to
-- libcrypto / BCryptGenRandom) is the standard option.

math.randomseed(os.time())

local hi = math.random(0, 0xFFFFFFFF)
local lo = math.random(0, 0xFFFFFFFF)
print(string.format("Random 64-bit number: %08x%08x", hi, lo))
