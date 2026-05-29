// AArch64 Linux: read a 64-bit value from the ARMv8.5-RNG instruction RNDR.
// On success RNDR clears NZCV (Z=0); on failure (briefly-depleted reservoir)
// RNDR sets Z=1. The Arm ARM recommends retrying on failure.
//
// Build: as -o RNDR.o RNDR.asm && ld -o RNDR RNDR.o
.global _start

.section .data
random_number: .space 8

.section .text
_start:
1:  mrs     x0, RNDR            // attempt to read a random 64-bit value
    b.eq    1b                  // Z=1 means failure -> retry
    adr     x1, random_number
    str     x0, [x1]            // store the random value

    mov     x8, 93              // sys_exit
    mov     x0, 0               // exit code
    svc     0