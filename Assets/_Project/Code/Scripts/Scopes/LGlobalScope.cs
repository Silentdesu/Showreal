using VContainer;

namespace TechnoDemo.Scopes
{
    public sealed class LGlobalScope : LBaseScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessageBrokers(builder, out var options);
            
            this.LogDIRegisterSuccess();
        }
    }
}
