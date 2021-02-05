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


mov edx, 0h
add edx, 30h
mov [num], edx
WHILE:
mov edx, cont
mov eax, 10
cmp edx, eax
jae END_WHILE
mov edx, 1h
add edx, 30h
mov ebx, edx
mov edx, 0h
add edx, 30h
mov ebx, edx
mov edx, ebx
add edx, 30h
mov [cont2], edx
mov edx, cont
mov eax, 5
cmp edx, eax
jae END_WHILE
mov edx, 1h
add edx, 30h
mov ebx, edx
mov edx, 0h
add edx, 30h
mov ebx, edx
mov edx, num
add edx, cont2
mov [T4], edx
mov edx, ebx
add edx, 30h
mov [num], edx
ELSE:
mov edx, 0h
add edx, 30h
mov [cont], edx
mov edx, cont
add edx, 1
mov [T5], edx
mov edx, ebx
add edx, 30h
mov [cont], edx
END_WHILE:
push 0
lea eax, [ebp-4]
push eax
push 1
push num
push ebx
call _WriteFile@20


push 0
call _ExitProcess@4


section .data

cont2 DB 0
contador DB 0
num DB 0
cont DB 0
