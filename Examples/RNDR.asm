.global _start

.section .data
random_number: .space 8

.section .text
_start:
    mrs x0, RNDR
    str x0, random_number
    
    mov x8, 93
    mov x0, 0
    svc 0