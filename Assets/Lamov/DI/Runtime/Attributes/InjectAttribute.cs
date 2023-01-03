using System;

namespace Lamov.DI.Runtime.Attributes 
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InjectAttribute : Attribute
    {   
        public Type Type => _type;
        
        private readonly Type _type = null;

        public InjectAttribute(Type type) 
        {
            _type = type;
        }
    }
}