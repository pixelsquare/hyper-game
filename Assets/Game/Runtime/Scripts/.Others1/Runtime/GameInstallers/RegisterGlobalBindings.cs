using System.Linq;
using ModestTree;
using Santelmo.Rinsurv.Backend;
using Zenject;

namespace Santelmo.Rinsurv
{
    /// <summary>
    /// Responsible for injecting bindings from any context to project context.
    /// Making it globally accessible. It only add new bindings but does not remove.
    /// </summary>
    public class RegisterGlobalBindings : IInitializable
    {
        private readonly DiContainer _diContainer;

        public RegisterGlobalBindings(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Initialize()
        {
            var projectContainer = ProjectContext.Instance.Container;

            foreach (var contract in _diContainer.AllContracts)
            {
                if (projectContainer.AllContracts.Contains(contract))
                {
                    continue;
                }

                var contractType = contract.Type;
                var instance = _diContainer.TryResolve(contractType);

                if (!contractType.DerivesFrom<IGlobalBinding>() && !contractType.DerivesFrom<IService>())
                {
                    continue;
                }

                projectContainer.Bind(contractType)
                                .FromInstance(instance)
                                .CopyIntoAllSubContainers()
                                .IfNotBound();
            }
        }
    }
}
