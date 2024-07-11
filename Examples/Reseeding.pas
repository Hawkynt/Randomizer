program GenerateRandom64Bit;

uses
  SysUtils;

function Generate64BitRandomNumber: UInt64;
begin
  Result := (UInt64(Random(MaxInt)) shl 32) or UInt64(Random(MaxInt));
end;

var
  RandomNumber: UInt64;
begin
  // Seed the random number generator with the current time
  Randomize;
  RandomNumber := Generate64BitRandomNumber;
  WriteLn('First 64-bit random number: ', RandomNumber);

  // Re-Seed the random number generator again with the current time
  Randomize;
  RandomNumber := Generate64BitRandomNumber;
  WriteLn('Second 64-bit random number: ', RandomNumber2);

  ReadLn;
end.