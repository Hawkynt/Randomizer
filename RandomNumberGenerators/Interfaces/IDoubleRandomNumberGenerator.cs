namespace Hawkynt.RandomNumberGenerators.Interfaces;

public interface IDoubleRandomNumberGenerator {
  double Next() {
    var (result, _) = this.NextPair();
    return result;
  }

  (double, double) NextPair() => (this.Next(), this.Next());
}
