﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lamov.DI.Runtime.Attributes;

namespace Lamov.DI.Runtime
{
    public class DiContainer
    {
        public DiContainer Parent { get; private set; }
        
        private readonly Dictionary<Type, BindInfo> _registered;
        
        public DiContainer(DiContainer parent = null)
        {
            Parent = parent;
            _registered = new Dictionary<Type, BindInfo>();
        }

        #region Resolve

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
            
            if (container.Parent != null) bindings.AddRange(GetAllValidBindings(container.Parent));
            
            return bindings;
        }
        
        private object Resolve(Type type)
        {
            if (!_registered.TryGetValue(type, out var bindInfo)) 
            {
                if (Parent == null)
                    throw new Exception($"Can't resolve. Object of type '{type.Name}' hasn't registered");

                return Parent.Resolve(type);
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