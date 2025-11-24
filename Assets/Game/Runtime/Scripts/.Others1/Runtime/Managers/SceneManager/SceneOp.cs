using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public class SceneOp : ISceneOp
    {
        public UniTask Task { get; private set; }

        public static SceneOp Create(UniTask task)
        {
            return new SceneOp { Task = task };
        }
    }
}
