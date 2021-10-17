.8086
.model small
.stack 100h
.code
main:
mov ax, DGROUP
mov ds, ax
mov ax, 4C00h
mov ax, 4
push ax
mov ax, 2
push ax
pop ax
pop bx
mov si, bx
mov c[si], ax
mov ax, 7
push ax
pop bx
push bx
mov ax, 2
cmp ax, bx
jae eeeennnndddd_1_0
jmp eeeennnndddd_0
eeeennnndddd_1_0:
pop di
mov ax, c[di]
push ax
pop ax
mov bx, 1
push ax
cmp ax, bx
je case_1_0
pop ax
mov bx, 2
push ax
cmp ax, bx
je case_1_0
pop ax
mov bx, 5
push ax
cmp ax, bx
je case_1_0
jmp case_1_0_end
case_1_0:
lea dx, var_1
mov ah, 9
int 21h
jmp case_0_end
case_1_0_end:
pop ax
mov bx, 3
push ax
cmp ax, bx
je case_2_0
pop ax
mov bx, 4
push ax
cmp ax, bx
je case_2_0
pop ax
mov bx, 6
push ax
cmp ax, bx
je case_2_0
jmp case_2_0_end
case_2_0:
lea dx, var_2
mov ah, 9
int 21h
jmp case_0_end
case_2_0_end:
pop ax
mov bx, 8
push ax
cmp ax, bx
je case_3_0
jmp case_3_0_end
case_3_0:
lea dx, var_3
mov ah, 9
int 21h
jmp case_0_end
case_3_0_end:
pop ax
mov bx, 10
push ax
cmp ax, bx
je case_4_0
pop ax
mov bx, 7
push ax
cmp ax, bx
je case_4_0
jmp case_4_0_end
case_4_0:
lea dx, var_4
mov ah, 9
int 21h
jmp case_0_end
case_4_0_end:
jmp case_0_end
eeeennnndddd_0:
lea dx, var_0
mov ah, 9
int 21h
case_0_end:
.data
@buffer db 6
blength db (?)
@buf db 256 DUP (?)
output db 6 DUP (?)
err_msg db  "Input error, try again", 0Dh, 0Ah, "$"
@true db "true"
@false db "false"
@@true db "true$"
@@false db "false$"
clear db 0Dh, 0Ah, "$"
f db (?)
g db (?)
h db 100 DUP (?)
a dw (?)
b dw (?)
c dw 2 DUP (?)
value dw 20 DUP (?)
flag db (?)
flag1 db 2 DUP (?)
var_1 db "case_1", 0Dh, 0Ah, "$"
var_2 db "case_2", 0Dh, 0Ah, "$"
var_3 db "case_3", 0Dh, 0Ah, "$"
var_4 db "case_4", 0Dh, 0Ah, "$"
var_0 db "CASE_ERROR", 0Dh, 0Ah, "$"
end main
