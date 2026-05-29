; ====================================================================
;  C64 6510 random-number demo
;
;    Part 1: TRNG by reading the SID chip's voice-3 noise oscillator
;            (the upper 8 bits of the noise generator appear at OSC3).
;    Part 2: Deterministic PRNG via a 16-bit Galois LFSR, seeded with
;            $0083 = 131 — the same idea many C64-era game cartridges
;            used when they needed predictable randomness for replays.
;
;  Assemble with the cc65 toolchain:
;    ca65 -t c64 -o SidNoise.o SidNoise.asm
;    ld65 -t c64 -o SidNoise.prg SidNoise.o c64.lib
;
;  Run on real hardware or in VICE:
;    LOAD "SIDNOISE",8,1 : RUN
; ====================================================================

CHROUT       = $FFD2              ; Kernal: write A to default output

; --- SID register addresses (chip base $D400) ---
SID_V3_FLO   = $D40E
SID_V3_FHI   = $D40F
SID_V3_CTRL  = $D412
SID_OSC3     = $D41B               ; read-only: upper 8 bits of voice 3 osc

.segment "CODE"

start:
    ; ---- Header: TRNG ----
    ldx #0
@h1:
    lda hdr1,x
    beq @sid_setup
    jsr CHROUT
    inx
    bne @h1

@sid_setup:
    ; Voice 3: maximum frequency, noise waveform (gate off — output is
    ; the raw noise generator, not the audible envelope).
    lda #$ff
    sta SID_V3_FLO
    sta SID_V3_FHI
    lda #$80
    sta SID_V3_CTRL

    ; Read 8 bytes from OSC3 with a short delay between reads so the
    ; noise generator has time to advance between samples.
    ldx #0
@sid_loop:
    ldy #16
@delay:
    dey
    bne @delay
    lda SID_OSC3
    jsr print_hex
    inx
    cpx #8
    bne @sid_loop
    lda #13
    jsr CHROUT

    ; ---- Header: PRNG ----
    ldx #0
@h2:
    lda hdr2,x
    beq @prng_seed
    jsr CHROUT
    inx
    bne @h2

@prng_seed:
    ; Seed the 16-bit LFSR with $0083 (= 131).
    lda #$83
    sta lfsr
    lda #$00
    sta lfsr+1

    ldx #0
@prng_loop:
    jsr lfsr_step
    lda lfsr                       ; emit low byte
    jsr print_hex
    inx
    cpx #8
    bne @prng_loop
    lda #13
    jsr CHROUT
    rts

; ----- 16-bit Galois LFSR (taps $B400, period 65535) -----
lfsr_step:
    lsr lfsr+1
    ror lfsr
    bcc @done
    lda lfsr+1
    eor #$B4
    sta lfsr+1
@done:
    rts

; ----- Print A as two PETSCII hex digits -----
print_hex:
    pha
    lsr a
    lsr a
    lsr a
    lsr a
    jsr nibble
    pla
    and #$0f
    jmp nibble                    ; tail-call: return goes to print_hex's caller

nibble:
    cmp #10
    bcc @digit
    clc
    adc #('A' - 10)
    jmp CHROUT
@digit:
    clc
    adc #'0'
    jmp CHROUT

.segment "RODATA"
hdr1:  .byte "TRNG (SID NOISE):  ", 0
hdr2:  .byte "PRNG (LFSR $0083): ", 0

.segment "BSS"
lfsr:  .res 2
