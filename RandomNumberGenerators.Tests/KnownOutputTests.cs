using System.Collections;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.QuasiRandom;

namespace RandomNumberGenerators.Tests;

[TestFixture]
public class KnownOutputTests {

  private class KnownOutputSource : IEnumerable {
    public IEnumerator GetEnumerator() {
      yield return Case("AdditiveCongruentialRandomNumberGenerator", new AdditiveCongruentialRandomNumberGenerator(), 0xC103763898D6DA00UL, 0x157F8E282269D6BDUL, 0xA369DD4A8D1C3A70UL);
      yield return Case("CombinedLinearCongruentialGenerator_Add", new CombinedLinearCongruentialGenerator(CombinationMode.Additive), 0xCF37BD15801FF901UL, 0x7ECF67C1DCBA2C8DUL, 0x64FF73C9A914F041UL);
      yield return Case("ComplementaryMultiplyWithCarry", new ComplementaryMultiplyWithCarry(), 0x9C7903E8379D2D3EUL, 0x242E9E003E52AD04UL, 0xA7793F68A9178E7CUL);
      yield return Case("FeedbackWithCarryShiftRegister", new FeedbackWithCarryShiftRegister(), 0x0000000000000083UL, 0x55293276E7A192C3UL, 0xF6442E22D544E452UL);
      yield return Case("InversiveCongruentialGenerator", new InversiveCongruentialGenerator(), 0x9DBA4EA6A7072A21UL, 0xD79D32F7FC827466UL, 0xCDE72E9B56C08120UL);
      yield return Case("KeepItSimpleStupid_Xor", new KeepItSimpleStupid(CombinationMode.Xor), 0x741A9D831767C7F6UL, 0x5C5D3B731E10C093UL, 0xFFD3EC9E3EC45EECUL);
      yield return Case("LaggedFibonacciGenerator_Add", new LaggedFibonacciGenerator(mode: CombinationMode.Additive), 0x6A830A58CAFEAB66UL, 0x3D0FF79C3E792BBEUL, 0x3A643F570997146DUL);
      yield return Case("LinearCongruentialGenerator", new LinearCongruentialGenerator(), 0x45F56EAD27E79556UL, 0x82F408F851406B6DUL, 0x3E38700017767678UL);
      yield return Case("LinearFeedbackShiftRegister", new LinearFeedbackShiftRegister(), 0x8000000000000041UL, 0x5D16E80000000029UL, 0xC49A92436F800011UL);
      yield return Case("MersenneTwister", new MersenneTwister(), 0xA6707773C7056CDEUL, 0xF2B7484C22930BB7UL, 0x6355DC69951FEF99UL);
      yield return Case("MiddleSquare", new MiddleSquare(), 0xFFFF77E000000000UL, 0xB79DFC0000000000UL, 0x77F800000147747CUL);
      yield return Case("MiddleSquareWeylSequence", new MiddleSquareWeylSequence(), 0xFFFF77E0B5AD4ECEUL, 0x9BD479F6DFBD549EUL, 0xC36C2BCF1626E3BCUL);
      yield return Case("Mixmax", new Mixmax(), 0xAD4CB18F03D6BD4EUL, 0xC03E8153EA284FBEUL, 0x0C923FABF1BAB4D1UL);
      yield return Case("MultiplicativeLinearCongruentialGenerator", new MultiplicativeLinearCongruentialGenerator(), 0x31EFF32E30801407UL, 0x68EB1AE6CC85FE3BUL, 0xA341F7DDE903F55FUL);
      yield return Case("MultiplyWithCarry", new MultiplyWithCarry(), 0x31EFF32E30801383UL, 0xDEA9338B4F706B35UL, 0xC1EB9C9E39FAAD8DUL);
      yield return Case("PermutedCongruentialGenerator", new PermutedCongruentialGenerator(), 0x57428F3665A96ADAUL, 0x8701F288295B028DUL, 0x745D0BCDECBDB36CUL);
      yield return Case("SplitMix64", new SplitMix64(), 0x8E48A85C3260D9D0UL, 0x8C7D277EC1AD250DUL, 0x26F7C914430F1465UL);
      yield return Case("SubtractWithBorrow", new SubtractWithBorrow(), 0x9B9E5E2108FB4413UL, 0xB3F283AC1F726290UL, 0x2C402ADB558E99D3UL);
      yield return Case("WellEquidistributedLongperiodLinear", new WellEquidistributedLongperiodLinear(), 0x8290AE57F0D98F7CUL, 0x861FECD503DE455DUL, 0x9C4239EF00C8F4EEUL);
      yield return Case("WichmannHill", new WichmannHill(), 0xDFF5C61DB3EBD21EUL, 0x891D5A076FD8E7C1UL, 0x208743469D96834EUL);
      yield return Case("Xoroshiro128PlusPlus", new Xoroshiro128PlusPlus(), 0x2DFE9078301B0F5BUL, 0xE97EFBFE1AF2A22BUL, 0xCC2992484A70D44AUL);
      yield return Case("XorShift", new XorShift(), 0x0000000000004123UL, 0x000000000020C0CBUL, 0x0000000010488519UL);
      yield return Case("XorShiftPlus", new XorShiftPlus(), 0xFFFFFFC04180203CUL, 0xFFFFFFC041003013UL, 0xE020CFFFBD97CFFFUL);
      yield return Case("XorShiftStar", new XorShiftStar(), 0x72D422D14050C977UL, 0x6A5547D807214A5FUL, 0xAA9431CB0E508B3EUL);
      yield return Case("XorWow", new XorWow(), 0xE7EAADD814A50E5BUL, 0xF0839D110163C3FBUL, 0x4E17A191C6BC5A63UL);
      yield return Case("Xoshiro256SS", new Xoshiro256SS(), 0xFFF8A405B7C1A7D7UL, 0x132BCE8921742F33UL, 0x98A42D8F14841ECAUL);
      yield return Case("BlumBlumShub", new BlumBlumShub(), 0xB20A277276A15109UL, 0x45A96D19E515708CUL, 0x19BB892F17CE24F8UL);
      yield return Case("ChaCha20", new ChaCha20(), 0xD41B5ECA5B7B0F05UL, 0x88817D3B2A32D661UL, 0xADC2B5560099BB55UL);
      yield return Case("BlumMicali", new BlumMicali(), 0xE121CB7466BE85B0UL, 0xB9805BE2A123D998UL, 0x7B97CB4B9AE1E298UL);
      yield return Case("SelfShrinkingGenerator", new SelfShrinkingGenerator(), 0x6D1D68C0B0D0B088UL, 0xAFFAF56C4EAD0073UL, 0xB999C2E84EE28264UL);
      yield return Case("Isaac", new Isaac(), 0xAC8CCFD849B6322BUL, 0x8882D778BB35C352UL, 0x8127ADE02666D588UL);
      yield return Case("Philox", new Philox(), 0xB2D3D45435ACB62CUL, 0xD86C3EFA787DEA7DUL, 0x5E92602954F26F1CUL);
      yield return Case("Threefry", new Threefry(), 0xCFF8C185D672A2BAUL, 0x75835506D7E8DFB0UL, 0xE991739EB8398AD7UL);
      yield return Case("Squares", new Squares(), 0xA8E97759EB053BB9UL, 0xF71022E89EDA29EEUL, 0x8CBCEEA5AC8FA2B8UL);
      yield return Case("JenkinsSmallFast", new JenkinsSmallFast(), 0xEAF583AC75FE9241UL, 0xE34049E25E8E65CFUL, 0x6F3FB9C484841844UL);
      yield return Case("RomuTrio", new RomuTrio(), 0x8E48A85C3260D9D0UL, 0x6FCD03FEFE554C97UL, 0x843EFFA262E44872UL);
      yield return Case("Lehmer128", new Lehmer128(), 0xA41A7F6B52EBE080UL, 0xB78364051D279C13UL, 0xDC09218676CCC65DUL);
      yield return Case("Lxm", new Lxm(), 0x6165DE57DB2ED6E9UL, 0x37DDB307E7B34037UL, 0xC9F6A736A52DD4CEUL);
      yield return Case("WyRand", new WyRand(), 0x820CA3278D64F00DUL, 0xE4E1790ED92ABF04UL, 0x09FA23BA39C7F0F6UL);
      yield return Case("Sfc64", new Sfc64(), 0xE327D8F1AE54CA0BUL, 0xA8FA61AB18280DD0UL, 0xFF716C05A65A7E9FUL);
      yield return Case("Mrg32k3a", new Mrg32k3a(), 0x6DE4A92AF647246CUL, 0xD036D8ADF77EC90DUL, 0x5CD69B9DE9A716F3UL);
      yield return Case("Trivium", new Trivium(), 0x228338D6AFDB0556UL, 0xB6FBE821E22481FCUL, 0x33EFFFB64E1B141FUL);
      yield return Case("Halton", new Halton(), 0x2100000000000000UL, 0xA100000000000000UL, 0x6100000000000000UL);
      yield return Case("Sobol", new Sobol(), 0x6300000000000000UL, 0xE300000000000000UL, 0xA300000000000000UL);
      yield return Case("Xoshiro256Plus", new Xoshiro256Plus(), 0x32FC8C1A9C2756C9UL, 0x11C4F1B672A39531UL, 0x493159B38AB76A9CUL);
      yield return Case("PermutedCongruentialXslRr", new PermutedCongruentialXslRr(), 0x098CB5B7F1CC0423UL, 0x10DC4C115DC45F5BUL, 0x719D2D24613163AFUL);
      yield return Case("AesCtrDrbg", new AesCtrDrbg(), 0x42490233F32F9891UL, 0xBCFFDA1227689B06UL, 0x2C6F04D69D6BDB41UL);
      yield return Case("HmacDrbg", new HmacDrbg(), 0x2104EEB29D817350UL, 0x52E9796B57443A8AUL, 0x49200D13CE16C422UL);
      yield return Case("AnsiX931", new AnsiX931(), 0xAD4DE494B7537389UL, 0xD8A8A03A02B4AAB3UL, 0x9C09EBF48F1D4936UL);
      yield return Case("Yarrow", new Yarrow(), 0x5AB3D45C41953D52UL, 0x4B8ED9CD632BB863UL, 0xCF722BA0BF99C4DFUL);
      yield return Case("Fortuna", new Fortuna(), 0x5AB3D45C41953D52UL, 0x4B8ED9CD632BB863UL, 0xCF722BA0BF99C4DFUL);
      yield return Case("Xoroshiro128Plus", new Xoroshiro128Plus(), 0x1AC5CFDAF40DFEDDUL, 0x4AE4B7D7265098D3UL, 0x3FBD8582D026680AUL);
      yield return Case("PermutedCongruentialXshRr", new PermutedCongruentialXshRr(), 0x43DD919109FABB3BUL, 0x3AAEF2C15A94C76FUL, 0x05FC56A98DFF86F5UL);
      yield return Case("Rule30", new Rule30(), 0x3155D0B72A000000UL, 0xF3A8B3F0003B5E4FUL, 0xD68A6D6CD785D1B5UL);
      yield return Case("Salsa20", new Salsa20(), 0x5ECA479EE25420B2UL, 0xBB8E5E65E7DBDDC7UL, 0xE2AB909709FA4C75UL);
      yield return Case("HashDrbg", new HashDrbg(), 0x210BFE7264F6807EUL, 0xC6BD500616C2C6AEUL, 0x4B2EC60FF6486E8CUL);
      yield return Case("ChaCha8", new ChaCha8(), 0x7AFAC6449B9A4AC4UL, 0x45E577F5E0FBD9ADUL, 0x3F0EB26CC7786C34UL);
      yield return Case("ChaCha12", new ChaCha12(), 0xE81860513AC7086CUL, 0xC3B35FFDB830E5DFUL, 0xB61A12D2BDCF6406UL);
      yield return Case("Salsa12", new Salsa12(), 0x1D1ACB2EDA7A448FUL, 0xE336E8383BA6D85AUL, 0xEE1A64E7436C877CUL);
      yield return Case("Salsa8", new Salsa8(), 0x4E02E4E005997ED3UL, 0xFDC652385C89209FUL, 0x7A930BB50F58E93AUL);
      yield return Case("Xoshiro128StarStar", new Xoshiro128StarStar(), 0xB7C1A80421742E01UL, 0x4ECD1AA460018F0CUL, 0x8B2C951FECD9FCB1UL);
      yield return Case("Xoshiro128PlusPlus", new Xoshiro128PlusPlus(), 0x460C3E9E079E341EUL, 0xAB5D65CC60422EC1UL, 0x881A7A9AD7487A95UL);
      yield return Case("Xoshiro128Plus", new Xoshiro128Plus(), 0x9C2756C9F4DB2567UL, 0xDA76AD2F36897902UL, 0xFA850E8D6749A5A1UL);
      yield return Case("Xoshiro512StarStar", new Xoshiro512StarStar(), 0xFFF8A405B7C1A7D7UL, 0x132BCE8921742F33UL, 0x6CAF949C9AC19F88UL);
      yield return Case("Xoshiro512PlusPlus", new Xoshiro512PlusPlus(), 0x09D8B3F41F7A7EE5UL, 0x4B692442CBB8D7D2UL, 0xC77F2212EE1F5B4CUL);
      yield return Case("Xoshiro512Plus", new Xoshiro512Plus(), 0xB5407170756FEE35UL, 0x850ED154E17D2D24UL, 0x643929478867765EUL);
      yield return Case("RomuQuad", new RomuQuad(), 0x8C7D277EC1AD250DUL, 0x41BCC848EF89A306UL, 0x4AC20EAD7D43D2B6UL);
      yield return Case("RomuDuo", new RomuDuo(), 0x8E48A85C3260D9D0UL, 0xFAAF404A982D45CFUL, 0x62247BA7B097FC5EUL);
      yield return Case("RomuDuoJr", new RomuDuoJr(), 0x8E48A85C3260D9D0UL, 0xFAAF404A982D45CFUL, 0x2CF978BAEF70A8F3UL);
      yield return Case("TinyMt", new TinyMt(), 0xAC417AB586A348BFUL, 0x6F2606B2C77E35AAUL, 0x4FC27A0CAEC8C075UL);
      yield return Case("Shishua", new Shishua(), 0x6E1C1D425C3F814DUL, 0xF6394AFAA8FD2F09UL, 0xC3668B888628592AUL);
      yield return Case("AsconPrf", new AsconPrf(), 0x5153222133BE478EUL, 0x986BF80CB580BAD4UL, 0x118E09D4F90DCD63UL);
    }

    private static TestCaseData Case(string name, IRandomNumberGenerator gen, ulong v1, ulong v2, ulong v3)
      => new TestCaseData(gen, v1, v2, v3).SetArgDisplayNames(name);
  }

  [TestCaseSource(typeof(KnownOutputSource))]
  public void First_Three_Values_Match_Known_Good_Output(IRandomNumberGenerator gen, ulong expected1, ulong expected2, ulong expected3) {
    gen.Seed(Generators.SEED);
    Assert.Multiple(() => {
      Assert.That(gen.Next(), Is.EqualTo(expected1), "value 1");
      Assert.That(gen.Next(), Is.EqualTo(expected2), "value 2");
      Assert.That(gen.Next(), Is.EqualTo(expected3), "value 3");
    });
  }
}
