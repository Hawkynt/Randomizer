// Swift exposes a cryptographically-strong source via `SystemRandomNumberGenerator`,
// which on Apple platforms wraps SecRandomCopyBytes / arc4random_buf and on Linux
// wraps /dev/urandom (getrandom on modern kernels). For lower-level access,
// `SecRandomCopyBytes` from Security.framework is available directly on Apple
// platforms.

import Foundation

var rng = SystemRandomNumberGenerator()
let random: UInt64 = rng.next()
print(String(format: "Random 64-bit number: %016llx", random))
