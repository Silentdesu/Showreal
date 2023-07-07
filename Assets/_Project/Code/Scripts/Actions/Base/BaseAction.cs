using System.Linq;

namespace TechnoDemo.Actions
{
    public abstract class BaseAction
    {
        public ActionData Data;

        protected readonly IActionHandler m_handler;
        protected bool m_isRunning;
        
        public BaseAction(IActionHandler handler)
        {
            m_handler = handler;
            SetData(m_handler.GetData(this));
            this.Log($"{GetType().Name} has created | AutoStart [{Data.AutoStart}]");
        }

        public virtual bool CanStart()
        {
            if (IsRunning()) return false;
            if (m_handler.ActiveActions == null) return true;
            
            System.Span<ActionTagSO> actionSpan = m_handler.ActiveActions;

            for (int i = 0, count = actionSpan.Length; i < count; i++)
            {
                if (Data.BlockedBy.Contains(m_handler.ActiveActions[i])) return false;
            }

            return true;
        }
        
        public virtual void Start()
        {
            this.Log($"Skill {GetType().Name} has started!");
            m_isRunning = true;
            m_handler.TrackAction(Data.Tag);
            m_handler.onActionStartedEvent?.Invoke(this);
        }

        public virtual void Stop()
        {
            this.Log($"Skill {GetType().Name} has stopped!");
            m_isRunning = false;
            m_handler.UntrackAction(Data.Tag);
            m_handler.onActionStoppedEvent?.Invoke(this);
        }

        public bool IsRunning() => m_isRunning;
        public void SetData(in ActionData data) => Data = data;
    }
}