using System;
using System.Collections.Generic;

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
    }
}