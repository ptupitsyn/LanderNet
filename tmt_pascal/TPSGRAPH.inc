(* Header fo TPSGRAPH UNIT *)
(* INTERFACE PART          *)
InterFace

 Type
      TSprite = Packed record
       W                  : DWord;
       H                  : DWord;
       Bpp                : DWord;
       RESERVED           : Array[0..6] of DWORD;
      End;

 Const
      SizeOfSprite = SizeOf(TSprite);
      ADDITIVE_BLENDING           = 1;
      MIXING_BLENDING             = 2;
      SHADOW_BLENDING             = 3;

 Var
    MMX_ENABLED      : Boolean;
    USE_BANKED       : Boolean;
    PagesToAlloc     : Byte = 1;
    ScreenSX         : DWord;
    ScreenSY         : DWord;
    ScreenBpp        : DWord;
    LFBMem           : DWord;
    TransparentColor : DWord = 0;
    pBIOSFont        : Pointer = Nil;
    pFont            : Pointer = Nil;

    {Drawing Procedures}
    PSET             : Procedure(X,Y,C:DWord);
    PGET             : Function (X,Y:DWord):DWord;
    hLine            : Procedure(X,Y,X2,C:DWord);
    vLine            : Procedure(X,Y,Y2,C:DWord);
    Box              : Procedure(X1,Y1,X2,Y2,C:DWord);
    FillBox          : Procedure(X1,Y1,X2,Y2,C:DWord);
    CLS              : Procedure(C:DWORD);
    RGB2Color        : Function (R,G,B:Byte):DWord;
    Color2RGB        : Procedure(Color:DWord; Var R,G,B:Byte);
    tPut             : Procedure(X,Y:LongInt;Arr:Pointer);  {Transparent PUT}
    btPut            : Procedure(X,Y:LongInt;Arr:Pointer; BlendMode:Byte);  {Blending PUT}
    PutRgn           : Procedure(X,Y,X1,Y1,X2,Y2:DWord; Arr:Pointer);
    PutChar          : Procedure(X,Y,C,S:DWord; Symbol:Char);
    vGradient        : Procedure(X1,Y1,X2,Y2,C1,C2:DWord);
    hGradient        : Procedure(X1,Y1,X2,Y2,C1,C2:DWord);
    Circle           : Procedure(X,Y,R,C:DWord);
    FillCircle       : Procedure(X,Y,R,C:DWord);
    bTransitLayer    : Procedure(pSpr:Pointer);



 { Mode Control Procedures }

 Procedure SetGRAPHMMX(ST:Boolean);

 Procedure DrawLayer(pBuf:Pointer);
 Procedure CopyLayer(pBuf1, pBuf2 :Pointer);
 Procedure SetLayer(pSpr:Pointer);
 Procedure ConvertBpp(pSpr:Pointer);
 Procedure GetSprSize(Var X,Y :DWord; Var PPic :Pointer);
 Procedure SetFont(pFnt:Pointer);
 Procedure PutString(X,Y,C,S:DWord;Text:String);