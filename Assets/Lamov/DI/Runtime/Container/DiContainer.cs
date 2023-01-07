using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lamov.DI.Runtime.Attributes;

namespace Lamov.DI.Runtime
{
    public class DiContainer
    {
        private DiContainer _parent;
        
        private readonly Dictionary<Type, BindInfo> _registered;
        
        public DiContainer(DiContainer parent = null)
        {
            _parent = parent;
            _registered = new Dictionary<Type, BindInfo>();
        }

        public void SetParent(DiContainer parentContainer) => _parent = parentContainer;

        #region Bind
        
        public BindInfo Bind<TInterface>(TInterface instance)
        {
            var bindInfo = new BindInfo(instance, typeof(TInterface));
            _registered[typeof(TInterface)] = bindInfo;

            return bindInfo;
        }

        public BindInfo Bind<TInt, TImpl>() where TImpl : class, TInt => Bind<TInt>((TImpl)NewInstance(typeof(TImpl)));
        
        public BindInfo Bind<T>() where T : class => Bind<T>((T)NewInstance(typeof(T)));
        
        
        #endregion

        #region Resolve
        
        public T Resolve<T>() => (T)Resolve(typeof(T));
        
        public void ResolveInMethodWithAttribute(Type type) 
        {
            var bindInfos = GetAllValidBindings(this);

            foreach (var bindInfo in bindInfos)
            {
                var attrType = typeof(InjectAttribute);
                var instance = bindInfo.Instance;
                
                var methods = instance.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(attrType, false).Length > 0)
                    .Where(m =>((InjectAttribute) m.GetCustomAttribute(attrType)).Type == type)
                    .ToArray();

                foreach (var info in methods) 
                {
                    var args = info.GetParameters()
                        .Select(param => Resolve(param.ParameterType))
                        .ToArray();

                    info.Invoke(instance, args);
                }
            }
        }
        
        private static List<BindInfo> GetAllValidBindings(DiContainer container)
        {
            var bindings = new List<BindInfo>();
            
            foreach (var (type, bindInfo) in container._registered) 
            {
                if(bindInfo.Instance == null) continue;

                bindings.Add(bindInfo);
            }
            
            if (container._parent != null) bindings.AddRange(GetAllValidBindings(container._parent));
            
            return bindings;
        }
        
        private object Resolve(Type type)
        {
            if (!_registered.TryGetValue(type, out var bindInfo)) 
            {
                if (_parent == null)
                    throw new Exception($"Can't resolve. Object of type '{type.Name}' hasn't registered");

                return _parent.Resolve(type);
            }

            if (bindInfo == null) throw new NullReferenceException($"Can't resolve. Object of type '{type.Name}' is null");

            if (bindInfo.LifeTime != DiBindLifeTime.Single) return NewInstance(type);

            return bindInfo.Instance ?? (bindInfo.Instance = NewInstance(type));
        }
        
        private object NewInstance(Type type) 
        {
            var ctor = type.GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .First();

            var args = ctor.GetParameters()
                .Select(param => Resolve(param.ParameterType))
                .ToArray();

            return Activator.CreateInstance(type, args);
        }

        #endregion
    }
}