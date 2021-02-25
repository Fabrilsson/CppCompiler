global _main


extern  _GetStdHandle@4
extern  _WriteFile@20
extern  _ExitProcess@4


section .text

_main:


mov ebp, esp
sub esp, 4


push -11
call _GetStdHandle@4
mov ebx, eax


mov [num], 0
mov ah, 1
mov ah, 0
cmp ah, 0
mov [cont2], T2
mov ah, 1
mov ah, 0
cmp ah, 0
mov [num], T4
mov [cont], 0
mov [cont], T5


push 0
call _ExitProcess@4


section .data

cont2 DB 0
contador DB 0
num DB 0
cont DB 0
