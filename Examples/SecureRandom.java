import java.security.SecureRandom;

/* SecureRandom is Java's cryptographically-strong PRNG. The no-arg constructor
   picks the JVM's default secure source (NativePRNG on Unix-likes,
   SHA1PRNG/Windows-PRNG depending on configuration). For NIST-compliant DRBG
   selection use SecureRandom.getInstance("DRBG"). */
public class SecureRandom_64Bit {
  public static void main(String[] args) {
    SecureRandom rng = new SecureRandom();
    long randomNumber = rng.nextLong();
    System.out.printf("Random 64-bit number: %016x%n", randomNumber);
  }
}
