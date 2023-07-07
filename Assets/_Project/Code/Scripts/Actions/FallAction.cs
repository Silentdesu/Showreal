using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;

namespace TechnoDemo.Actions
{
    public sealed class FallAction : BaseAction, ISetuper<FallAction>, IUpdateTickable
    {
        public FallAction(IActionHandler handler) : base(handler)
        {
        }

        public FallAction Setup(in IPlayer player)
        {
            return this;
        }

        public void UpdateTick(in IInput input)
        {
        }
    }
}