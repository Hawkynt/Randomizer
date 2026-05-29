// Modern C# (.NET 6+) cryptographic RNG. RandomNumberGenerator.Fill draws
// from the OS-level CSPRNG (BCryptGenRandom on Windows, getrandom/dev/urandom
// on Linux, SecRandomCopyBytes on macOS). For non-cryptographic use,
// System.Random.Shared exposes a process-wide thread-safe instance.

using System;
using System.Security.Cryptography;

Span<byte> buffer = stackalloc byte[8];
RandomNumberGenerator.Fill(buffer);
ulong random = BitConverter.ToUInt64(buffer);

Console.WriteLine($"Random 64-bit number (cryptographic):     {random:X16}");
Console.WriteLine($"Random 64-bit number (non-cryptographic): {Random.Shared.NextInt64():X16}");
