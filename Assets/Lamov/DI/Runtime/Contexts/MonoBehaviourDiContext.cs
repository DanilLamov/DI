using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lamov.DI.Runtime.Contexts
{
    public abstract class MonoBehaviourDiContext<TDiContext> : MonoBehaviour, IDiContext
    {
        public DiContainer Container { get; private set; }
        
        private async UniTaskVoid Awake()
        {
            Container = new DiContainer();

            await Bind();

            Container.ResolveInMethodWithAttribute(typeof(TDiContext));
        }

        protected abstract UniTask Bind();
    }
}