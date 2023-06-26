using System.Linq;

namespace TechnoDemo.Skills
{
    public abstract class Skill
    {
        public SkillData Data;

        protected readonly ISkillHandler m_handler;
        protected bool m_isRunning;
        
        public Skill(ISkillHandler handler)
        {
            m_handler = handler;
            SetData(m_handler.GetData(this));
            this.Log($"{GetType().Name} has created | AutoStart [{Data.AutoStart}]");
        }

        public virtual bool CanStart()
        {
            if (IsRunning()) return false;
            if (m_handler.ActiveSkills == null) return true;
            
            System.Span<SkillTagSO> skillsSpan = m_handler.ActiveSkills;

            for (int i = 0, count = skillsSpan.Length; i < count; i++)
            {
                if (Data.BlockedBy.Contains(m_handler.ActiveSkills[i])) return false;
            }

            return true;
        }
        
        public virtual void Start()
        {
            this.Log($"Skill {GetType().Name} has started!");
            m_isRunning = true;
            m_handler.TrackSkill(Data.Tag);
            m_handler.onSkillStartedEvent?.Invoke(this);
        }

        public virtual void Stop()
        {
            this.Log($"Skill {GetType().Name} has stopped!");
            m_isRunning = false;
            m_handler.UntrackSkill(Data.Tag);
            m_handler.onSkillStoppedEvent?.Invoke(this);
        }

        public bool IsRunning() => m_isRunning;
        public void SetData(in SkillData data) => Data = data;
    }
}