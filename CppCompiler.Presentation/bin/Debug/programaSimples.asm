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


mov ah, 0
mov [num], ah
WHILE:
cmp DWORD [cont], 10
jge LS3
mov DWORD [T0], 1
je LS5
LS3:
mov DWORD [T0], 0
LS5:
cmp DWORD [T0], 0
je END_WHILE 
mov ah, [T2]
mov [cont2], ah
cmp DWORD [cont], 5
jge LS15
mov DWORD [T3], 1
je LS17
LS15:
mov DWORD [T3], 0
LS17:
cmp DWORD [T3], 0
je ELSE 
mov ah, [T4]
mov [num], ah
ELSE:
mov ah, 0
mov [cont], ah
mov ah, [T5]
mov [cont], ah
je WHILE 
END_WHILE:


push 0
call _ExitProcess@4


section .data

cont2 DQ 0
contador DQ 0
num DQ 0
cont DQ 0
T5 DQ 0
T4 DQ 0
T3 DQ 0
T2 DQ 0
T1 DQ 0
T0 DQ 0
