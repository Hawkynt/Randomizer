Imports System

Module Program
  Sub Main()
    Dim seed As Integer = DateTime.Now.Ticks And Integer.MaxValue
    Dim rand As New Random(seed)
    Console.WriteLine("Random 64-bit number: " & rand.NextInt64())
  End Sub
End Module