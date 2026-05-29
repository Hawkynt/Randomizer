% MATLAB: rand/randi/randn use Mersenne Twister by default (rng()) — fine
% for Monte Carlo and simulation, NOT cryptographically secure. For
% crypto-grade randomness use java.security.SecureRandom via MATLAB's
% Java interop (MATLAB ships a JVM).
%
% Set rng() with a fixed seed for reproducible runs.

rng('default');  % Mersenne Twister, seed 0

% MATLAB's `rand` returns doubles in [0, 1); cast to 64-bit unsigned int via
% two 32-bit halves to avoid the 53-bit double-precision loss.
hi = randi([0 intmax('uint32')], 1, 1, 'uint32');
lo = randi([0 intmax('uint32')], 1, 1, 'uint32');
n  = bitshift(uint64(hi), 32) + uint64(lo);
fprintf('Random 64-bit number (Mersenne Twister): %016x\n', n);

% Crypto-grade alternative via Java:
sr = java.security.SecureRandom();
b  = zeros(1, 8, 'int8');
sr.nextBytes(b);
% Java bytes come back signed; reinterpret to unsigned and combine.
m = uint64(0);
for k = 1:8
    m = bitor(bitshift(m, 8), uint64(typecast(int8(b(k)), 'uint8')));
end
fprintf('Random 64-bit number (SecureRandom):     %016x\n', m);
