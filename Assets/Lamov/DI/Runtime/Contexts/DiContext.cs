using UnityEngine;

namespace Lamov.DI.Runtime.Contexts
{
    public class DiContext<TContext> : Singleton<TContext> where TContext : class, new()
    {
        public DiContainer Container { get; }
        
        public DiContext() 
        {
            Container = new DiContainer();
        }
        
        public T Instantiate<T>(T original, Transform parent) where T : Object
        {
            var instance = Object.Instantiate(original, parent);
            Container.Bind(instance);
            Container.ResolveInMethodWithAttribute(typeof(TContext));
            return instance;
        }
    }
}