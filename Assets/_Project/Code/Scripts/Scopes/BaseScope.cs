using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Scopes
{
    public abstract class BaseScope : LifetimeScope
    {
        protected virtual void RegisterMessageBrokers(in IContainerBuilder builder, out MessagePipeOptions options)
        {
            options = builder.RegisterMessagePipe();
            
            builder.RegisterBuildCallback(resolver => GlobalMessagePipe.SetProvider(resolver.AsServiceProvider()));
        }
    }
}