namespace Randomizer.Deterministic;

public class MultiplyWithCarry : IRandomNumberGenerator {
  private const ulong A = 6364136223846793005UL;  // Multiplier
  private ulong _state;                           // Current state
  private ulong _carry;                           // Carry value

  public void Seed(ulong seed) {
    _state = seed;
    _carry = ~seed;
  }

  public ulong Next() { // implicit mod 2^64
    // Split _state into high and low 32-bit parts
    ulong lowState = _state & 0xFFFFFFFF;
    ulong highState = _state >> 32;

    // Split A into high and low 32-bit parts
    ulong lowA = A & 0xFFFFFFFF;
    ulong highA = A >> 32;

    // Calculate the product parts
    ulong lowProduct = lowState * lowA;
    ulong midProduct1 = lowState * highA;
    ulong midProduct2 = highState * lowA;
    ulong highProduct = highState * highA;

    // Combine the product parts and add the carry
    ulong carryPart = (lowProduct >> 32) + (midProduct1 & 0xFFFFFFFF) + (midProduct2 & 0xFFFFFFFF) + _carry;
    ulong resultLow = (lowProduct & 0xFFFFFFFF) | ((carryPart & 0xFFFFFFFF) << 32);
    ulong resultHigh = highProduct + (midProduct1 >> 32) + (midProduct2 >> 32) + (carryPart >> 32);

    // Update the state and carry
    _state = resultLow;
    _carry = resultHigh >> 32;  // Use the high part of resultHigh as the new carry

    return _state;
  }
}