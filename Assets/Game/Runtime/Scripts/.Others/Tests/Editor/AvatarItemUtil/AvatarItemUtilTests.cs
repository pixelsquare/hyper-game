using Kumu.Kulitan.Avatar;
using NUnit.Framework;
using UnityEditor;

namespace Kumu.Kulitan.Editor.Tests
{
    public class AvatarItemUtilTests
    {
        [Test]
        public void HasItemTypeOverlap_ReturnsTrue_WhenItemTypesAreDuplicated()
        {
            var assets = new[]
            {
                AssetDatabase.LoadAssetAtPath<AvatarItemConfig>("Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData/UB_tshirt_muscletees_01_G.asset"),
                AssetDatabase.LoadAssetAtPath<AvatarItemConfig>("Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData/UB_tshirt_oversized_01_P.asset"),
                AssetDatabase.LoadAssetAtPath<AvatarItemConfig>("Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData/LB_longpants_rippedjeans_01.asset"),
            };

            Assert.That(AvatarItemUtil.HasItemTypeOverlap(assets), Is.True);
        }
        
        [Test]
        public void HasItemTypeOverlap_ReturnsFalse_WhenItemTypesAreDistinct()
        {
            var assets = new[]
            {
                AssetDatabase.LoadAssetAtPath<AvatarItemConfig>("Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData/UB_tshirt_muscletees_01_G.asset"),
                AssetDatabase.LoadAssetAtPath<AvatarItemConfig>("Assets/_Source/Content/ScriptableObjects/Portal/Avatars/ItemDatabase/ItemData/LB_longpants_rippedjeans_01.asset"),
            };

            Assert.That(AvatarItemUtil.HasItemTypeOverlap(assets), Is.False);
        }
    }
}
