using MessagePipe;
using TechnoDemo.Actions;
using TechnoDemo.Core;
using TechnoDemo.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Scopes
{
    [DisallowMultipleComponent]
    public sealed class SceneScope : BaseScope
    {
        [SerializeField] private SceneScopeContainerSO m_containerDataSo;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessageBrokers(builder, out var options);
            
            builder.RegisterEntryPoint<GameManager>().As<IGameManager>().WithParameter(m_containerDataSo);
            builder.Register<ActionHandler>(Lifetime.Scoped).As<IActionHandler>().WithParameter(m_containerDataSo.ActionContainerSo);
            
            this.LogDIRegisterSuccess();
        }

        protected override void RegisterMessageBrokers(in IContainerBuilder builder, out MessagePipeOptions options)
        {
            base.RegisterMessageBrokers(in builder, out options);

            builder.RegisterMessageBroker<JumpMessage>(options);
            builder.RegisterMessageBroker<GroundMessage>(options);
        }
    }
}