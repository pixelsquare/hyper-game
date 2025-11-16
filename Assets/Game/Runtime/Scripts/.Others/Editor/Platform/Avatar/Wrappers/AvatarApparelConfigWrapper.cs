using System;
using UnityEngine;  

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarApparelConfigWrapper<T> : AvatarItemConfigEditorWrapper
        where T : AvatarApparelConfig
    {
        protected abstract string GetMaterialName();

        public override AvatarItemConfig CreateConfig(string guid, string filename)
        {   
            var config = ScriptableObject.CreateInstance<T>();
            
            CreateData(config, guid, filename);
            
            var matName = GetMaterialName();
            CreateAssetRefMaterial(config, matName);
            
            CreateAssetRefSprite(config, filename);
            
            CreateAssetRefMesh(config, filename);
            
            CreateAssetRefTexture(config, filename);

            return config;
        }

        protected override Type GetConfigType()
        {
            return typeof(T);
        }
    }

    public class EyewearConfigWrapper : AvatarApparelConfigWrapper<AvatarEyewearConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_EW";
        }
    }

    public class FaceAccConfigWrapper : AvatarApparelConfigWrapper<AvatarFaceAccessoryConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_FA";
        }
    }

    public class FootwearConfigWrapper : AvatarApparelConfigWrapper<AvatarFootwearConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_FW";
        }
    }

    public class FullBodyConfigWrapper : AvatarApparelConfigWrapper<AvatarFullBodyConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_FB";
        }
    }

    public class HairConfigWrapper : AvatarApparelConfigWrapper<AvatarHairConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_HAIR";
        }
    }

    public class HandAccessoryWrapper : AvatarApparelConfigWrapper<AvatarHandAccessoryConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_HD";
        }
    }

    public class HeadwearConfigWrapper : AvatarApparelConfigWrapper<AvatarHeadwearConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_HW";
        }
    }

    public class LowerBodyConfigWrapper : AvatarApparelConfigWrapper<AvatarLowerBodyConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_LB";
        }
    }

    public class NoseConfigWrapper : AvatarApparelConfigWrapper<AvatarNoseConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_Nose";
        }
    }

    public class SocksConfigWrapper : AvatarApparelConfigWrapper<AvatarSocksConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_SK";
        }
    }

    public class UpperAccConfigWrapper : AvatarApparelConfigWrapper<AvatarUpperAccessoryConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_UA";
        }
    }

    public class UpperBodyConfigWrapper : AvatarApparelConfigWrapper<AvatarUpperBodyConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_AVATAR_UB";
        }
    }
}
