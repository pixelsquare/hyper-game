using System.Collections.Generic;
using System.Linq;

namespace Kumu.Kulitan.Avatar
{
    public static class AvatarItemUtil
    {
        public static AvatarItemType ToAvatarItemType(string typecode)
        {
            switch (typecode)
            {
                case "B":
                    return AvatarItemType.SkinColor;
                case "EB":
                    return AvatarItemType.Eyebrows;
                case "E":
                    return AvatarItemType.Eyes;
                case "EW":
                    return AvatarItemType.Eyewear;
                case "FA":
                    return AvatarItemType.Face;
                case "FW":
                    return AvatarItemType.Shoes;
                case "FB":
                    return AvatarItemType.FullBody;
                case "H":
                    return AvatarItemType.Hair;
                case "HD":
                    return AvatarItemType.Hand;
                case "HW":
                    return AvatarItemType.HeadWear;
                case "LB":
                    return AvatarItemType.LowerClothing;
                case "M":
                    return AvatarItemType.Mouth;
                case "N":
                    return AvatarItemType.Nose;
                case "SD":
                    return AvatarItemType.SkinDesign;
                case "SM":
                    return AvatarItemType.SkinMark;
                case "SK":
                    return AvatarItemType.Socks;
                case "UA":
                    return AvatarItemType.UpperAccessory;
                case "UB":
                    return AvatarItemType.UpperClothing;
                default:
                    return AvatarItemType.SkinColor;
            }
        }

        public static string ToTypeCode(AvatarItemType itemType)
        {
            switch (itemType)
            {
                case AvatarItemType.Eyebrows:
                    return "EB";
                case AvatarItemType.Eyes:
                    return "E";
                case AvatarItemType.Eyewear:
                    return "EW";
                case AvatarItemType.Face:
                    return "FA";
                case AvatarItemType.Shoes:
                    return "FW";
                case AvatarItemType.FullBody:
                    return "FB";
                case AvatarItemType.Hair:
                    return "H";
                case AvatarItemType.Hand:
                    return "HD";
                case AvatarItemType.HeadWear:
                    return "HW";
                case AvatarItemType.LowerClothing:
                    return "LB";
                case AvatarItemType.Mouth:
                    return "M";
                case AvatarItemType.Nose:
                    return "N";
                case AvatarItemType.SkinDesign:
                    return "SD";
                case AvatarItemType.SkinMark:
                    return "SM";
                case AvatarItemType.Socks:
                    return "SK";
                case AvatarItemType.UpperAccessory:
                    return "UA";
                case AvatarItemType.UpperClothing:
                    return "UB";
                case AvatarItemType.SkinColor:
                default:
                    return "B";
            }
        }

        public static bool HasItemTypeOverlap(IEnumerable<AvatarItemConfig> collection)
        {
            return collection.GroupBy(i => i.GetTypeCode())
                             .Any(g => g.Count() > 1);
        }
    }
}
