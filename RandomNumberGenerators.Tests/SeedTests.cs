using Hawkynt.RandomNumberGenerators.Interfaces;

namespace RandomNumberGenerators.Tests;

[TestFixture]
public class SeedTests {

  [TestCaseSource(typeof(Generators.AllGeneratorsSource))]
  public void Same_Seed_Produces_Same_Sequence(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var first = new ulong[20];
    for (var i = 0; i < first.Length; ++i)
      first[i] = gen.Next();

    gen.Seed(Generators.SEED);
    for (var i = 0; i < first.Length; ++i)
      Assert.That(gen.Next(), Is.EqualTo(first[i]), $"Mismatch at index {i} after re-seeding");
  }

  [TestCaseSource(typeof(Generators.AllGeneratorsSource))]
  public void Different_Seeds_Produce_Different_First_Value(IRandomNumberGenerator gen) {
    gen.Seed(131);
    var v1 = gen.Next();

    gen.Seed(137);
    var v2 = gen.Next();

    Assert.That(v1, Is.Not.EqualTo(v2));
  }
}
