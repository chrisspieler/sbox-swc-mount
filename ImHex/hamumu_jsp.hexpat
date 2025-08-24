struct SpriteInfo {
    u16 Width;
    u16 Height;
    u16 OffsX;
    u16 OffsY;
    u32 Size;
    u32 Unk0;
};

u16 SpriteCount @ 0x0;
SpriteInfo SpriteHeaders[SpriteCount] @ 0x2;