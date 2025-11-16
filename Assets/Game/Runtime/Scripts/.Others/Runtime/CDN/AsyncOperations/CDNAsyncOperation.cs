using UnityEngine;

namespace Kumu.Kulitan.CDN
{
    public abstract class CDNAsyncOperation : CustomYieldInstruction
    {
        public virtual bool IsDone => Progress >= 1f;

        public abstract bool IsFailed { get; }

        public abstract object Result { get; }

        public virtual float Progress { get; protected set; } = 0.0f;

        public override bool keepWaiting => Progress < 1f;

        public abstract void StartOp();
    }
}
