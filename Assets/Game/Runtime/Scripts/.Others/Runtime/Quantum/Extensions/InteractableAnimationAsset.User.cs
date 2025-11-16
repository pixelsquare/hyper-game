using UnityEngine;

public partial class InteractableAnimationAsset
{
    [SerializeField] private InteractableTag interactableTag = new() { tag = "default" };

    public InteractableTag InteractableTag => interactableTag;
}
