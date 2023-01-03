using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lamov.DI.Runtime.Contexts
{
    public abstract class RootDiContext<TDiContext> : MonoBehaviour where TDiContext : DiContext<TDiContext>, new()
    {
        protected DiContext<TDiContext> _diContext;
        
        private async UniTaskVoid Awake()
        {
            var attr = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static;
            _diContext = (TDiContext)typeof(TDiContext).GetProperty("Instance", attr).GetValue(null);

            await Bind();

            _diContext.Container.ResolveInMethodWithAttribute(typeof(TDiContext));
        }

        protected abstract UniTask Bind();
    }
}