using System;

namespace Kumu.Kulitan.Avatar
{
    [Flags]
    public enum AvatarItemType
    {
        None            = 0b0000000000000000000,
        SkinColor       = 0b0000000000000000001, // body
        SkinDesign      = 0b0000000000000000010, // body
        Hair            = 0b0000000000000000100, // body
        Eyebrows        = 0b0000000000000001000, // body
        Eyes            = 0b0000000000000010000, // body
        Nose            = 0b0000000000000100000, // body
        Mouth           = 0b0000000000001000000, // body
        SkinMark        = 0b1000000000000000000, // body
        UpperClothing   = 0b0000000000100000000, // clothing
        LowerClothing   = 0b0000000001000000000, // clothing
        FullBody        = 0b0000000010000000000, // clothing
        Shoes           = 0b0000000100000000000, // clothing
        Socks           = 0b0000010000000000000, // clothing
        HeadWear        = 0b0000000000010000000, // accessory
        Hand            = 0b0000001000000000000, // accessory
        Face            = 0b0000100000000000000, // accessory
        Wrist           = 0b0001000000000000000, // accessory
        Eyewear         = 0b0010000000000000000, // accessory
        UpperAccessory  = 0b0100000000000000000, // accessory
        AllBody = 0b1000000000001111111,
        AllClothing = 0b0000010111100000000,
        AllAccessory = 0b0111101000010000000,
    }
}
