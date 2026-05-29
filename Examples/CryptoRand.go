package main

import (
	"crypto/rand"
	"encoding/binary"
	"fmt"
	"os"
)

// Go's crypto/rand is the standard cryptographically-secure source. It reads
// from /dev/urandom on Linux/macOS, BCryptGenRandom on Windows, getrandom(2) on
// modern Linux kernels, etc. — picking the best available OS entropy source
// automatically.
func main() {
	var buf [8]byte
	if _, err := rand.Read(buf[:]); err != nil {
		fmt.Fprintln(os.Stderr, "Failed to generate random bytes:", err)
		os.Exit(1)
	}
	n := binary.LittleEndian.Uint64(buf[:])
	fmt.Printf("Random 64-bit number: %016x\n", n)
}
