using VContainer;

namespace TechnoDemo.Scopes
{
    public sealed class GlobalScope : BaseScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessageBrokers(builder, out var options);
            
            this.LogDIRegisterSuccess();
        }
    }
}
