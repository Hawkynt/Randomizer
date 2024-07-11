use strict;
use warnings;

my $random_number;
open my $fh, '<', '/dev/urandom' or die "Can't open /dev/urandom: $!";
  binmode $fh;
  read $fh, $random_number, 8; # Read 64-bit random number
close $fh;

print "Random 64-bit number: ";
foreach my $byte (split //, $random_number) {
  printf "%02x", ord($byte);
}
print "\n";