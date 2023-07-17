using System;
using MessagePipe;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;

namespace TechnoDemo.Actions
{
    public sealed class FallAction : BaseAction, ISetuper<FallAction>, IUpdateTickable, IDisposable
    {
        private readonly IDisposable m_disposable;

        private float m_verticalVelocity;
        
        public FallAction(IActionHandler handler, ISubscriber<JumpMessage> jumpSubscriber) : base(handler)
        {
            var disposableBag = DisposableBag.CreateBuilder();
            jumpSubscriber.Subscribe(OnJumpMessageReceive).AddTo(disposableBag);
            m_disposable = disposableBag.Build();
        }

        public FallAction Setup(in IPlayer player)
        {
            return this;
        }

        public void UpdateTick(in IInput input)
        {
            
        }

        private void OnJumpMessageReceive(JumpMessage message)
        {
            m_verticalVelocity = message.VerticalVelocity;
        }

        public void Dispose()
        {
            m_disposable?.Dispose();
        }
    }
}