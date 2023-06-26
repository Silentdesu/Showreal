using MessagePipe;
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
            
            this.LogDIRegisterSuccess();
        }
    }
}