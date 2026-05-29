// Node.js: the `crypto` module's randomBytes / randomInt / randomFillSync
// are cryptographically secure (OpenSSL-backed). Distinct from the browser
// example LatencySource.js — Node.js has direct OS entropy access.
//
// Run: node NodeCrypto.js

const { randomBytes, randomInt } = require('node:crypto');

// 8 raw bytes -> 64-bit number
const buf = randomBytes(8);
const n = buf.readBigUInt64LE();
console.log(`Random 64-bit number: ${n.toString(16).padStart(16, '0')}`);

// Cryptographically-secure uniform integer in [0, N):
const ranged = randomInt(0, Number.MAX_SAFE_INTEGER);
console.log(`Random int < 2^53:    ${ranged}`);
