using System;

namespace Lamov.DI.Runtime
{
    public class BindInfo
    {
        public object Instance { get; set; }
        public Type Type { get; set; }
        public DiBindLifeTime LifeTime { get; set; }

        public BindInfo(object instance, Type type, DiBindLifeTime lifeTime = DiBindLifeTime.Single)
        {
            Instance = instance;
            Type = type;
            LifeTime = lifeTime;
        }
    }
}