// Zig (0.11+): std.crypto.random is the cryptographically-secure source,
// while std.Random.DefaultPrng provides a fast (Xoshiro256++) generator
// for non-cryptographic use.
//
// Build: zig build-exe Random.zig

const std = @import("std");

pub fn main() !void {
    const stdout = std.io.getStdOut().writer();

    // Cryptographic source — picks the OS-best CSPRNG at startup.
    const crypto_value = std.crypto.random.int(u64);
    try stdout.print("Random 64-bit number (cryptographic):     {x:0>16}\n", .{crypto_value});

    // Non-cryptographic (Xoshiro256++) seeded from std.crypto.random:
    var prng = std.Random.DefaultPrng.init(std.crypto.random.int(u64));
    const fast_value = prng.random().int(u64);
    try stdout.print("Random 64-bit number (non-cryptographic): {x:0>16}\n", .{fast_value});
}
