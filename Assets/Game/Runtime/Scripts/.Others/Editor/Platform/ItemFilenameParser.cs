namespace Kumu.Kulitan.Avatar
{
    public static class ItemFilenameParser
    {
        public static AvatarItemConfigEditorWrapper ToItemConfig(string typecode, string filename) 
        {
            switch (typecode)
            {
                case "B":
                    return new BodyConfigWrapper();
                case "EB":
                    return new EyebrowConfigWrapper();
                case "E":
                    
                    if (filename.EndsWith("_base"))
                    {
                        return new EyeConfigWrapper();
                    }

                    return null;
                
                case "EW":
                    return new EyewearConfigWrapper();
                case "FA":
                    return new FaceAccConfigWrapper();
                case "FW":
                    return new FootwearConfigWrapper();
                case "FB":
                    return new FullBodyConfigWrapper();
                case "H":
                    return new HairConfigWrapper();
                case "HD":
                    return new HandAccessoryWrapper();
                case "HW":
                    return new HeadwearConfigWrapper();
                case "LB":
                    return new LowerBodyConfigWrapper();
                case "M":
                    return new MouthConfigWrapper();
                case "N":
                    return new NoseConfigWrapper();
                case "SD":
                    return new SkinDesignConfigWrapper();
                case "SM":
                    return new SkinMarkConfigWrapper();
                case "SK":
                    return new SocksConfigWrapper();
                case "UA":
                    return new UpperAccConfigWrapper();
                case "UB":
                    return new UpperBodyConfigWrapper();
                default:
                    return null;
            }
        }
    }
}
