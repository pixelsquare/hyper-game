using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Legacy/Layer Config")]
    public class LegacyLayerConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<LegacySlot, GameObject> _actives;
        [SerializeField] private GameObject[] _passives;

        public bool TryRollPassive(out GameObject legacy)
        {
            if (_passives.Length < 1)
            {
                legacy = null;
                return false;
            }
            
            var random = Random.Range(0, _passives.Length);
            legacy = _passives[random];
            return true;
        }

        public bool TryGetActive(LegacySlot slot, out GameObject gameObject)
        {
            var hasValue = _actives.TryGetValue(slot, out gameObject);
            return hasValue && gameObject;
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {   
            if (!_actives.TryGetValue(LegacySlot.Strike, out var strike)
                || !strike
                || !strike.TryGetComponent<ILegacy>(out var legacyStrike)
                || legacyStrike.LegacySlot != LegacySlot.Strike)
            {
                _actives[LegacySlot.Strike] = null;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            if (!_actives.TryGetValue(LegacySlot.Move, out var move)
                || !move
                || !move.TryGetComponent<ILegacy>(out var legacyMove)
                || legacyMove.LegacySlot != LegacySlot.Move)
            {
                _actives[LegacySlot.Move] = null;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            if (!_actives.TryGetValue(LegacySlot.Deflect, out var deflect)
                || !deflect
                || !deflect.TryGetComponent<ILegacy>(out var legacyDeflect)
                || legacyDeflect.LegacySlot != LegacySlot.Deflect)
            {
                _actives[LegacySlot.Deflect] = null;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            if (!_actives.TryGetValue(LegacySlot.Cast, out var cast)
                || !cast
                || !cast.TryGetComponent<ILegacy>(out var legacyCast)
                || legacyCast.LegacySlot != LegacySlot.Cast)
            {
                _actives[LegacySlot.Cast] = null;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            if (!_actives.TryGetValue(LegacySlot.Smite, out var smite)
                || !smite
                || !smite.TryGetComponent<ILegacy>(out var legacySmite)
                || legacySmite.LegacySlot != LegacySlot.Smite)
            {
                _actives[LegacySlot.Smite] = null;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif
    }
}
