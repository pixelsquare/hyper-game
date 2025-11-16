using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public interface IOrderableTransform
    {
        int Order { get; set; }
        Transform GetTransform();
        Transform GetTransformParent();
    }
}
