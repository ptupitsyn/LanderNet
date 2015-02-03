{8bit Routines for TPSGRAPH}

 Procedure SetupPalette; Assembler;
  Asm
   Cli
   {Set Index}
    Mov Dx, 3C8h
    Mov Al, 0
    Out Dx, Al
    Mov Dx, 3C9h;
   {Set Colors}
    Mov Ecx, 256
    @SetNext:
     Mov Ebx, 256
     Sub Ebx, Ecx
     {R}
     Mov Eax, Ebx
     Shr Al, 5
     //And Al, 00000111b
     Shl Al, 3
     Or Al, 00000111b
     Out Dx, Al
     {G}
     Mov Eax, Ebx
     Shr Al, 3
     //And Al, 00000111b
     Shl Al, 3
     Or Al, 00000111b
     Out Dx, Al
     {B}
     Mov Eax, Ebx
     //And Al, 00000011b
     Shl Al, 4
     Or Al, 00001111b
     Out Dx, Al
    Dec Ecx; Jnz @SetNext
   Sti
  End;

 Procedure PSET8(X,Y,C:DWord); Assembler;
   Asm
    { Put Pixel }
    Mov Eax, Y
    Mul ScreenSX
    Mov Edi, LFBMem
    Add Edi, X
    Add Edi, Eax
    Mov Eax, C
    StosB
    { End }
   End;

 Function PGET8(X,Y:DWord):DWord; Assembler;
   Asm
    { Get Pixel }
    Mov Eax, Y
    Mul ScreenSX
    Mov Esi, LFBMem
    Add Esi, X
    Add Esi, Eax
    Xor Eax, Eax
    LodsB
    Mov Ah, Al
    Mov Bx, Ax
    Shl Eax, 16
    Mov Ax, Bx
    { End }
   End;

 Procedure hLine8(X,Y,X2,C:DWord); Assembler;
   Asm
    Mov Eax, Y
    Mul ScreenSX
    Mov Edi, X
    Mov Ecx, Edi
    Add Edi, LFBMem
    Add Edi, Eax
    Mov Eax, C
    Sub Ecx, X2
    Neg Ecx
    ALIGN 4
    Rep StosB
   End;

 Procedure vLine8 (X, Y1, Y2, C  :DWORD);Assembler;
   Asm
    Mov Edi, LFBMem
    Mov Eax, Y1
    Mov Ebx, ScreenSX
    Mul Ebx
    Add Eax, X
    Add Edi, Eax {EDI=Start Loct}
    Dec Ebx      {EBX=Dif}
    Mov Eax, C
    Mov Ecx, Y2
    Sub Ecx, Y1  {CX=DeltaY}
    ALIGN 4
    @Next:
     StosB
     Add Edi, Ebx
    Dec Ecx; Jnz @Next
   End;

 Procedure FillBox8(X1,Y1,X2,Y2,C:DWord);Assembler;
   Asm
    Mov Edi, LFBMem
    Mov Eax, ScreenSX
    Mov Ebx, Y1
    Mul Ebx
    Add Eax, X1
    Add Edi, Eax     //Edi=Beg Pos
    Mov Edx, Y2
    Sub Edx, Y1          //Edx=SY
    Inc Edx
    Mov Eax, C              //Al=Color
    Mov Ecx, X2
    Sub Ecx, X1
    Mov Ebx, ScreenSX
    Sub Ebx, Ecx              //Ebx=Diff
    @NextLine:
     Push Ecx
     Rep StosB
     Pop Ecx
     Add Edi, Ebx
    Dec Edx; Jnz @NextLine
   End;

 Procedure tPut8(X,Y:LongInt;Arr:Pointer);Assembler;  {Transparent PUT}
    Var IMSX, IMSY :DWord;
   Asm
    Cmp Arr, 0
    Je @ExitSub

    {Check ON-SCREEN POS}
    Mov Eax, ScreenSY; Mov Ebx, ScreenSX
    Cmp Y, Eax; Jl @PUT1; Jmp @ExitSub; @PUT1:
    Cmp X, Ebx; Jl @PUT2; Jmp @ExitSub; @PUT2:
    {--------}
    Mov Edi, LFBMem  {Set Destination Loct}
    {Get Sizes}
    Mov Esi, Arr
    LodsD; Mov IMSX, Eax
    LodsD; Mov IMSY, Eax
    Add Esi, SizeOfSprite-8
    {Check ON-SCREEN POS}
    Mov Eax, IMSY; Neg Eax; Cmp Eax, Y; Jl @PUT3; Jmp @ExitSub; @PUT3:
    Mov Eax, IMSX; Neg Eax; Cmp Eax, X; Jl @PUT4; Jmp @ExitSub; @PUT4:
    {VERTICAL Clipping}
    Mov Eax, Y    {Clipping Bottom}
    Add Eax, IMSY
    Cmp Eax, ScreenSY
    Jl @SkipClipYB
     Sub Eax, ScreenSY
     Cmp Eax, IMSY
     Jl @DoClipYB
     Jmp @ExitSub
     @DoClipYB:
     Sub IMSY, Eax
    @SkipClipYB:
    Cmp Y, -1           {Clipping Top}
    Jg @SkipClipYT
     Xor Eax, Eax
     Sub Eax, Y
     Cmp Eax, IMSY
     Jl @DoClipYT
     Jmp @ExitSub
     @DoClipYT:
     Sub IMSY, Eax
     Add Y, Eax
     Mov Ebx, IMSX
     Mul Ebx
     Add Esi, Eax
    @SkipClipYT:
    {End Clipping}

    {Calculate Destination MemLocation}
    Mov Eax, Y; Mov Ebx, ScreenSX;
    Mul Ebx
    Add Edi, Eax
    Add Edi, X

    Mov Ecx, IMSY {Size Y}
    Mov Ebx, IMSX {Size X}
    Mov Edx, ScreenSX
    Sub Edx, Ebx

    {HORIZ.CLIPPING}
    Push Edx
    Xor Eax, Eax
    {RIGHT}
    Sub Edx, X
    Cmp Edx, 0
    Jge @NoClip1   {IF DX>=0 THEN JUMP}
     Mov Eax, Edx; Neg Eax; Sub Ebx, Eax
    @NoClip1:
    Pop Edx
    {LEFT}
    Cmp X, 0
    Jge @NoClip2
     Sub Edi, X; Sub Esi, X
     Sub Eax, X; Sub Ebx, Eax
    @NoClip2:

    ALIGN 4
    @PutLn:  {DRAW!!!!!}
     Push Ecx; Push Eax; Mov Ecx, Ebx
     ALIGN 4
     @PutDot:
      LodsB; Cmp Al, Byte(TransparentColor) //Test Al, Al
      Je @NextDot  {if Al==0}
       StosB; Dec Edi
      @NextDot: Inc Edi
     Dec Ecx; Jnz @PutDot  {Looping is SLOW}
     Pop Eax; Add Esi, Eax
     Add Edi, Edx; Add Edi, Eax
     Pop Ecx
    Dec Ecx; Jnz @PutLn    {Looping is SLOW}

    @ExitSub:

   End;

 Procedure btPut8(X,Y:LongInt;Arr:Pointer; BlendMode:Byte);Assembler;  {Transparent PUT}
    Var IMSX, IMSY :DWord;
   Asm
    Cmp Arr, 0
    Je @ExitSub

    {Check ON-SCREEN POS}
    Mov Eax, ScreenSY; Mov Ebx, ScreenSX
    Cmp Y, Eax; Jl @PUT1; Jmp @ExitSub; @PUT1:
    Cmp X, Ebx; Jl @PUT2; Jmp @ExitSub; @PUT2:
    {--------}
    Mov Edi, LFBMem  {Set Destination Loct}
    {Get Sizes}
    Mov Esi, Arr
    LodsD; Mov IMSX, Eax
    LodsD; Mov IMSY, Eax
    Add Esi, SizeOfSprite-8
    {Check ON-SCREEN POS}
    Mov Eax, IMSY; Neg Eax; Cmp Eax, Y; Jl @PUT3; Jmp @ExitSub; @PUT3:
    Mov Eax, IMSX; Neg Eax; Cmp Eax, X; Jl @PUT4; Jmp @ExitSub; @PUT4:
    {VERTICAL Clipping}
    Mov Eax, Y    {Clipping Bottom}
    Add Eax, IMSY
    Cmp Eax, ScreenSY
    Jl @SkipClipYB
     Sub Eax, ScreenSY
     Cmp Eax, IMSY
     Jl @DoClipYB
     Jmp @ExitSub
     @DoClipYB:
     Sub IMSY, Eax
    @SkipClipYB:
    Cmp Y, -1           {Clipping Top}
    Jg @SkipClipYT
     Xor Eax, Eax
     Sub Eax, Y
     Cmp Eax, IMSY
     Jl @DoClipYT
     Jmp @ExitSub
     @DoClipYT:
     Sub IMSY, Eax
     Add Y, Eax
     Mov Ebx, IMSX
     Mul Ebx
     Add Esi, Eax
    @SkipClipYT:
    {End Clipping}

    {Calculate Destination MemLocation}
    Mov Eax, Y; Mov Ebx, ScreenSX;
    Mul Ebx
    Add Edi, Eax
    Add Edi, X

    Mov Ecx, IMSY {Size Y}
    Mov Ebx, IMSX {Size X}
    Mov Edx, ScreenSX
    Sub Edx, Ebx

    {HORIZ.CLIPPING}
    Push Edx
    Xor Eax, Eax
    {RIGHT}
    Sub Edx, X
    Cmp Edx, 0
    Jge @NoClip1   {IF DX>=0 THEN JUMP}
     Mov Eax, Edx; Neg Eax; Sub Ebx, Eax
    @NoClip1:
    Pop Edx
    {LEFT}
    Cmp X, 0
    Jge @NoClip2
     Sub Edi, X; Sub Esi, X
     Sub Eax, X; Sub Ebx, Eax
    @NoClip2:

    Cmp MMX_ENABLED, 0
    Je @ExitSub2

    ALIGN 4
    @PutLn:  {DRAW!!!!!}
     Push Edx; Push Ecx; Push Eax; Mov Ecx, Ebx
     ALIGN 4
     @PutDot:
       Xor Eax, Eax
       Mov Al, Byte ptr [Esi]
       Mov Edx, Eax
       Shl Ax, 3
       Shl Ah, 5
       Shl Eax, 8
       Mov Al, Ah
       Shr Ah, 5
       Shl Ah, 5
       Shl Al, 3
       MovD mm0, Eax {Source color}

       Xor Eax, Eax
       Mov Al, Byte ptr [Edi]
       Mov Edx, Eax
       Shl Ax, 3
       Shl Ah, 5
       Shl Eax, 8
       Mov Al, Ah
       Shr Ah, 5
       Shl Ah, 5
       Shl Al, 3
       MovD mm1, Eax {Destination color}

       {Do Blending}
       Cmp BlendMode, 1  {Additive}
       Je @Additive
       Cmp BlendMode, 2  {Substractive}
       Je @Substractive
       Cmp BlendMode, 3  {HalfAdditive}
       Je @HalfAdditive
       Jmp @EndBlend
       {}
       @Additive:
        pAddUSB mm0, mm1
        MovD Eax, mm0
       Jmp @EndBlend
       @Substractive:
        pSubUSB mm1, mm0
        MovD Eax, mm1
       Jmp @EndBlend
       @HalfAdditive:
        pUnpckLBW mm2, mm0
        pUnpckLBW mm3, mm1
        pSrLW mm2, 1
        pSrLW mm3, 1
        PackSsWB mm0, mm2
        PackSsWB mm1, mm3
        pADDUSB mm0, mm1
        MovD Eax, mm0
       Jmp @EndBlend
       {--}
       @EndBlend:
       Mov Edx, Eax
       Shr Ah, 5
       Shl Ax, 2
       Shr Eax, 8
       Shr Ah, 5
       Shl Al, 3
       Shr Ax, 3
       StosB
       Add Esi, 1
       //Add Edi, 2
     Dec Ecx; Jnz @PutDot  {Looping is SLOW}
     Pop Eax; Pop Ecx; Pop Edx
     Add Esi, Eax
     Add Edi, Edx;
     Add Edi, Eax
    Dec Ecx; Jnz @PutLn    {Looping is SLOW}

    @ExitSub:
    Emms
    @ExitSub2:
   End;

 Procedure PutRgn8(X,Y,X1,Y1,X2,Y2:DWord; Arr:Pointer);Assembler;
    Var
       IMSX,IMSY,XX1,YY1,XX2,YY2,DX1,DX2,SX,SY :DWord;
   Asm
    Cmp Arr, 0
    Je @ExitSub
    //
    {Check Need for drawing}
     Mov Eax, X
     Cmp Eax, ScreenSX
     JGE @ExitSub

     Mov Eax, Y
     Cmp Eax, ScreenSY
     JGE @ExitSub

     Mov Eax, X
     Add Eax, X2
     Sub Eax, X1
     Cmp Eax, 0
     Jle @ExitSub

     Mov Eax, Y
     Add Eax, Y2
     Sub Eax, Y1
     Cmp Eax, 0
     Jle @ExitSub
    {Check Left Border}
     Mov Eax, X1
     Mov XX1, Eax
     Mov Eax, X
     Cmp Eax, 0
     Jg @Next1
     Sub XX1, Eax
     Sub X, Eax
     @Next1:
    {Check Upper Border}
     Mov Eax, Y1
     Mov YY1, Eax
     Mov Eax, Y
     Cmp Eax, 0
     Jg @Next2
     Sub YY1, Eax
     Sub Y, Eax
     @Next2:
    {Check Right Border}
     Mov Eax, X2
     Mov XX2, Eax
     Add Eax, X
     Sub Eax, X1
     Cmp Eax, ScreenSX
     Jl @Next3
     Sub Eax, ScreenSX
     Sub XX2, Eax
     @Next3:
    {Check Lower Border}
     Mov Eax, Y2
     Mov YY2, Eax
     Add Eax, Y
     Sub Eax, Y1
     Cmp Eax, ScreenSY
     Jl @Next4
     Sub Eax, ScreenSY
     Sub YY2, Eax
     @Next4:
    {Get Sizes}
     Mov Esi, Arr
     LodsD; Mov IMSX, Eax
     LodsD; Mov IMSY, Eax
     Mov Eax, XX2; Sub Eax, XX1; Mov SX, Eax
     Mov Eax, YY2; Sub Eax, YY1; Mov SY, Eax
    {Calc MemLocs}
     Mov Edi, LFBMem
     Mov Eax, Y
     Mul ScreenSX
     Add Eax, X
     Add Edi, Eax   {Dest Starting loct}
     Mov Eax, YY1
     Mul IMSX
     Add Esi, SizeOfSprite-8
     Add Esi, Eax
     Add Esi, XX1    {Src Starting Loct}
    {Calc Difs}
     Mov Eax, IMSX; Sub Eax, SX; Mov DX1, Eax    {Src}
     Mov Eax, ScreenSX; Sub Eax, SX; Mov DX2, Eax{Dest}

     Cmp MMX_ENABLED, 1
     Je @DoMMX
    {DRAW!!!!!}
     Mov Eax, SX
     Shr Eax, 2
     Mov Ecx, Eax
     Shl Eax, 2
     Neg Eax
     Add Eax, SX
     Mov Ebx, Eax
     Mov Eax, Ecx
     Mov Edx, SY

     //ALIGN 4
     @NextLine:
      Mov Ecx, Eax
      ALIGN 4
      Rep MovsD
      Mov Ecx, Ebx
      Rep MovsB
      Add Esi, DX1
      Add Edi, DX2
     Dec Edx; Jnz @NextLine
     Jmp @ExitSub

     @DoMMX:
    {DRAW!!!!!}
     Mov Eax, SX
     Shr Eax, 3
     Mov Ecx, Eax
     Shl Eax, 3
     Neg Eax
     Add Eax, SX
     Mov Ebx, Eax
     Mov Eax, Ecx
     Mov Edx, SY
     @NextLineMMX:
      Mov Ecx, Eax
      @NextMMXP:
       //ALIGN 4
       MovQ mm0, [esi]
       MovQ [edi], mm0
       Add Esi, 8
       Add Edi, 8
      Dec Ecx; Jg @NextMMXP
      Mov Ecx, Ebx
      Rep MovsB
      Add Esi, DX1
      Add Edi, DX2
     Dec Edx; Jnz @NextLineMMX
     Emms

    {Exit Sub}
    @ExitSub:
    //
   End;(**************)

 Procedure PutChar8(X,Y,C,S:DWord; Symbol:Char);Assembler;
    Var
       DX1: DWord;
   Asm
    Mov Esi, pFont
    Movzx Eax, Symbol
    Shl Eax, 4
    Add Esi, Eax {Char bitmap}

    Mov Edi, LFBMem
    Mov Eax, Y
    Mul ScreenSX
    Add Eax, X
    {<>}
    Add Edi, Eax

    Mov Eax, ScreenSX
    Sub Eax, 8
    {<>}
    Mov DX1, Eax

    Xor Ecx, Ecx
    Xor Eax, Eax
    Mov Cl, 16

    @NextTex:
     Mov Ebx, 128
     LodsB
     Mov Bh, Al
     Mov Ch, 8
     @NextDot:
      Mov Dl, Bh
      And Dl, Bl
      Cmp Dl, 0
      Je @Skip
      Mov Eax, C
      StosB
      Dec Edi
      @Skip:
      Inc Edi
      Shr Bl, 1
     Dec Ch; Jnz @NextDot
     Add Edi, DX1
    Dec Cl; Jnz @NextTex
    {}
   End;