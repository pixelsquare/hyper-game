using Kumu.Kulitan.Hangout;
using UnityEngine;

public abstract partial class AnimationDataAsset
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite displayIcon;

    [SerializeField] private string animParamKey;
    [SerializeField] private AnimatorControllerParameterType animParamType = AnimatorControllerParameterType.Bool;

#if UNITY_EDITOR
    [SerializeField] private AnimationClip animationClipEditor;
#endif

    [SerializeField] [HideInInspector] private float animationLength;

    [SerializeField] private AudioClipConfig clipConfig;

    public string DisplayName => displayName;
    public Sprite DisplayIcon => displayIcon;

    public string AnimParamKey => animParamKey;
    public AnimatorControllerParameterType AnimParamType => animParamType;

#if UNITY_EDITOR
    public AnimationClip AnimationClipEditor => animationClipEditor;
#endif

    public float AnimationLength => animationLength;

    public AudioClipConfig ClipConfig => clipConfig;

#if UNITY_EDITOR
    public void SetAnimLengthDirty()
    {
        animationLength = animationClipEditor != null
                ? animationClipEditor.length
                : 0.0f;
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private void OnEnable()
    {
        SetAnimLengthDirty();
    }

    private void OnValidate()
    {
        SetAnimLengthDirty();
    }
#endif
}
