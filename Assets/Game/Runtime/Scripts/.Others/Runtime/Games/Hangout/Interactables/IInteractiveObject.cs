using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public interface IInteractiveObject
    {
        Transform Transform { get; }
        EntityRef EntityRef { get;  }
        
        void OnTryInteract();
        bool IsAvailable();
    }
}
