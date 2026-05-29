// F# (.NET): same RandomNumberGenerator API as the C# example, but with a
// functional flavour. Run as a script:  dotnet fsi RandomNumberGenerator.fsx

open System
open System.Security.Cryptography

let buffer = Array.zeroCreate<byte> 8
RandomNumberGenerator.Fill(buffer)
let cryptoValue = BitConverter.ToUInt64(buffer, 0)
printfn $"Random 64-bit number (cryptographic):     {cryptoValue:X16}"

// Non-cryptographic alternative (System.Random.Shared, .NET 6+):
let fastValue = Random.Shared.NextInt64()
printfn $"Random 64-bit number (non-cryptographic): {fastValue:X16}"
