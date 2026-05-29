// Kotlin (JVM): SecureRandom is the same as Java's java.security.SecureRandom,
// backed by the platform's OS entropy source. For Kotlin Multiplatform code
// you can use kotlin.random.Random.Default (non-cryptographic, fast) or
// kotlinx-crypto's SecureRandom for crypto-strong portability.

import java.nio.ByteBuffer
import java.security.SecureRandom

fun main() {
    val rng = SecureRandom()
    val bytes = ByteArray(8).also { rng.nextBytes(it) }
    val n = ByteBuffer.wrap(bytes).long
    println("Random 64-bit number: ${"%016x".format(n)}")
}
