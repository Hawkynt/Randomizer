using System.Diagnostics;
using Randomizer;

ulong seedNumber = 540;
var generator = new MiddleSquare();

var alreadySeen = new HashSet<ulong>();
var counter = 0;
ulong number;
var timer = Stopwatch.StartNew();
do {
  number = generator.Next();
  Console.WriteLine($"#{counter++}: {number}");
} while (!alreadySeen.Add(number));
timer.Stop();

Console.WriteLine($"We seeded with {seedNumber} and have repeated ourselves after {counter} steps with {number} which took {timer.ElapsedMilliseconds}ms ({counter/timer.Elapsed.TotalSeconds} per second).");
