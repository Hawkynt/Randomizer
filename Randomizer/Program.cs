using System.Diagnostics;
using Randomizer.Deterministic;

var generator = new AdditiveCongruentialRandomNumberGenerator();
const ulong seedNumber = 131;
generator.Seed(seedNumber);

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
