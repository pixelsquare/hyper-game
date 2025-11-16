using System.Reflection;
using System.Text.RegularExpressions;

namespace Kumu.Kulitan.Avatar
{
    public static class AvatarEditorConstants
    {
        public const string FIELD_DATA = "data";
        public const string FIELD_MESH_REF = "meshRef";
        public const string FIELD_MATERIAL_REF = "materialRef";
        public const string FIELD_SPRITE_REF = "spriteRef";
        public const string FIELD_TEXTURE_REF = "textureRef";

        public const string DIRECTORY_ITEM_MATS = "Assets/Submodules/kumu-ube-art-assets/Avatar/(3)Materials";
        public const string DIRECTORY_ITEM_MESH = "Assets/Submodules/kumu-ube-art-assets/Avatar/(1)Models/Items";
        public const string DIRECTORY_ITEM_ICONS = "Assets/Submodules/kumu-ube-art-assets/Avatar/(5)Preview/(2)Sprites";
        public const string DIRECTORY_ITEM_TEX = "Assets/Submodules/kumu-ube-art-assets/Avatar/(2)Textures/Items";
        public const string DIRECTORY_ITEM_CONFIGS = "Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData";

        public const string ADDR_GROUP_AVATAR_TEXTURES = "AvatarItemTextures";
        public const string ADDR_GROUP_AVATAR_MODELS = "AvatarItemModels";
        public const string ADDR_GROUP_AVATAR_MATS = "AvatarMaterials";
        public const string ADDR_GROUP_AVATAR_ICONS = "AvatarItemPreviewIcons";
        public const string ADDR_GROUP_AVATAR_CONFIGS = "AvatarItemConfigs";

        public const string PATH_ITEM_DATABASE =
            "Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/AvatarItemDatabase.asset";

        public const BindingFlags BINDING_CONFIG_FIELDS = BindingFlags.NonPublic | BindingFlags.Instance;
        
        public static readonly Regex PATTERN_FILENAME = new Regex(@"\w+_\w*_\w*_[0-9]+");
        public static readonly Regex PATTERN_ICON_NAME = new Regex(@"^\w+_\w*_\w*_[0-9]+(_.*)*_preview$");
        public static readonly Regex PATTERN_EYE_NAME = new Regex(@"^E_\w*_\w*_[0-9]+(_.*)*_(base|mask)$");
        
        public static readonly string[] ADDR_LABEL_AVATARS = new[] { "default", "kumu-avatars" };
    }
}
