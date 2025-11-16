using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/Services")]
    public abstract class UnitServiceBase<T> : WaitUnit where T : ResultBase
    {
        [DoNotSerialize]
        public ValueOutput successOutput;

        [DoNotSerialize]
        public ValueOutput error;

        [DoNotSerialize]
        public ControlOutput errorTrigger;

        protected Flow flow;
        protected Task<ServiceResultWrapper<T>> task;

        protected VariableDeclarations objectDeclarations;
        protected VariableDeclarations sceneDeclarations;

        protected abstract Task<ServiceResultWrapper<T>> GetServiceOperation();

        protected override void Definition()
        {
            base.Definition();
            successOutput = ValueOutput<ResultBase>(nameof(successOutput));
            error = ValueOutput<ServiceError>(nameof(error));
            errorTrigger = ControlOutput(nameof(errorTrigger));
            
            Succession(enter, errorTrigger);
        }

        protected sealed override IEnumerator Await(Flow flow)
        {
            this.flow = flow;
            objectDeclarations = Variables.Object(flow.stack.gameObject);
            sceneDeclarations = Variables.Scene(flow.stack.scene);

            task = GetServiceOperation();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasError)
            {
                flow.SetValue(error, task.Result.Error);
                yield return errorTrigger;
                yield break;
            }

            flow.SetValue(successOutput, task.Result.Result);
            BeforeExit();
            yield return exit;
        }

        protected virtual void BeforeExit()
        {
            // Keep empty
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T GetObjectVariable<T>(string varName)
        {
            return objectDeclarations.Get<T>(varName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T GetSceneVariable<T>(string varName)
        {
            return sceneDeclarations.Get<T>(varName);
        }
    }
}
