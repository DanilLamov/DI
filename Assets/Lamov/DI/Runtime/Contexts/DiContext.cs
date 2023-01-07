namespace Lamov.DI.Runtime.Contexts
{
    public class DiContext<TContext> : Singleton<TContext>, IDiContext where TContext : class, new()
    {
        public DiContainer Container { get; }
        
        public DiContext() 
        {
            Container = new DiContainer();
        }
    }
}