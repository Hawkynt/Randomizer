namespace Randomizer.Deterministic;

public class CombinedLinearCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state1;
  private ulong _state2;

  private const ulong _A1 = 6364136223846793005;  // Multiplier for LCG1
  private const ulong _C1 = 1442695040888963407;  // Increment  for LCG1
  
  private const ulong _A2 = 3935559000370003845;  // Multiplier for LCG2
  private const ulong _C2 = 2691343689449507681;  // Increment  for LCG2
  
  public void Seed(ulong seed) {
    this._state1 = seed;
    this._state2 = seed ^ 0x5DEECE66D; // Ensure different seeds for the two LCGs
  }

  public ulong Next() // implicit mod 2^64
    => (this._state1 = _A1 * this._state1 + _C1) 
       + (this._state2 = _A2 * this._state2 + _C2)
  ;
    
}
