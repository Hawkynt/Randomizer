# Ruby's SecureRandom (stdlib) draws from /dev/urandom on Unix-likes and
# BCryptGenRandom on Windows. The convenience helpers (hex, base64, uuid)
# are built on top of the same secure source.

require 'securerandom'

# Raw 8 bytes (= 64 bits) as a hex string:
puts "Random 64-bit number: #{SecureRandom.hex(8)}"

# Or as an integer:
n = SecureRandom.bytes(8).unpack1('Q<')   # 'Q<' = little-endian uint64
puts "As integer:           #{n}"
