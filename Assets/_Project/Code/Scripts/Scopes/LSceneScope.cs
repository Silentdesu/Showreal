using TechnoDemo.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Scopes
{
    [DisallowMultipleComponent]
    public sealed class LSceneScope : LBaseScope
    {
        [SerializeField] private SceneScopeContainerDataSO m_containerDataSo;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessageBrokers(builder, out var options);
            
            builder.RegisterEntryPoint<CGameManager>().As<IGameManager>().WithParameter(m_containerDataSo.GameManagerDataSo);
            
            this.LogDIRegisterSuccess();
        }
    }
}