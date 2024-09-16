using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.NonUniform;
using Randomizer;

//BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchy>();
//return;
var benchy = new Benchy();
benchy.MeasureThroughput();
return;

var generator = new ArbitraryNumberGenerator(new BlumMicali());
const ulong seedNumber = 131;
generator.Seed(seedNumber);

var bytes = generator.ConcatGenerator(1<<10);
var concatHex = bytes.ToHex();
var concatBin = bytes.ToBin();
var aes = generator.CipherGenerator(Aes.Create()).Take(8192).ToArray().ToHex();

var z = new InverseTransformSampling(generator);
var histogram = new ulong[256];
for (var i = 0; i < 1000000; ++i) {
  double random;
  do {
    random = z.Next() / 3.72;
  } while (random is < -1 or > 1);
  random=++random*0.5;

  var limited=(int)(random * 256);
  ++histogram[limited];
}

var values = Enumerable.Range(0, 16).Select(_ => generator.Mask16(0b11100011100111111001111)).ToArray();

var alreadySeen = new HashSet<ulong>();
var counter = 0;
ulong number;
var timer = new Stopwatch();
var lastStats = Stopwatch.StartNew();
do {
  timer.Start();
  number = generator.Next();
  timer.Stop();

  if (lastStats.Elapsed.TotalSeconds > 0.25) {
    Console.WriteLine($"#{counter}: {number} which took {timer.ElapsedMilliseconds}ms ({counter / timer.Elapsed.TotalSeconds:#,###.0} per second).");
    lastStats.Restart();
  } 
  
  ++counter;
} while (alreadySeen.Add(number));
timer.Stop();

Console.WriteLine($"We seeded with {seedNumber} and have repeated ourselves after {counter} steps with {number} which took {timer.ElapsedMilliseconds}ms ({counter / timer.Elapsed.TotalSeconds} per second).");
