namespace Randomizer;

public class CombinedLinearCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state1;
  private ulong _state2;

  private const ulong _a1 = 6364136223846793005;  // Multiplier for LCG1
  private const ulong _c1 = 1442695040888963407;  // Increment  for LCG1
  
  private const ulong _a2 = 3935559000370003845;  // Multiplier for LCG2
  private const ulong _c2 = 2691343689449507681;  // Increment  for LCG2
  
  public void Seed(ulong seed) {
    this._state1 = seed;
    this._state2 = seed ^ 0x5DEECE66D; // Ensure different seeds for the two LCGs
  }

  public ulong Next() // implicit mod 2^64
    => (this._state1 = (CombinedLinearCongruentialGenerator._a1 * this._state1 + CombinedLinearCongruentialGenerator._c1)) 
       + (this._state2 = (CombinedLinearCongruentialGenerator._a2 * this._state2 + CombinedLinearCongruentialGenerator._c2))
  ;
    
}
