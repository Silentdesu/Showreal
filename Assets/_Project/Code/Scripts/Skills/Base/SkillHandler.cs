using System;
using System.Collections.Generic;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;
using VContainer;

namespace TechnoDemo.Skills
{
    public interface ISkillHandler
    {
        IPlayer Player { get; }
        SkillTagSO[] ActiveSkills { get; }
        IUpdateTickable[] UpdateTickSkills { get; }
        
        Action<Skill> onSkillStartedEvent { get; }
        Action<Skill> onSkillStoppedEvent { get; }

        void AddSkill(in Skill skill);
        void RemoveSkill(in Skill skill);
        void TrackSkill(in SkillTagSO tag);
        void UntrackSkill(in SkillTagSO tag);
        void StartSkillByName(in string name);
        void StopSkillByName(in string name);
        SkillData GetData(in Skill skill);
    }

    public sealed class SkillHandler : ISkillHandler
    {
        public IPlayer Player { get; private set; }
        public SkillTagSO[] ActiveSkills { get; private set; }
        public IUpdateTickable[] UpdateTickSkills { get; private set; }

        public Action<Skill> onSkillStartedEvent { get; set; }
        public Action<Skill> onSkillStoppedEvent { get; set; }

        private SkillContainerSO m_container;
        
        private readonly List<Skill> m_totalSkills = new List<Skill>(10);
        private readonly List<SkillTagSO> m_activeSkills = new List<SkillTagSO>(3);
        private readonly List<IUpdateTickable> m_updateTickSkills = new List<IUpdateTickable>(3);

        [Inject]
        public SkillHandler(SkillContainerSO container)
        {
            m_container = container;
        }
        
        public void AddSkill(in Skill skill)
        {
            if (m_totalSkills.Contains(skill)) return;
            m_totalSkills.Add(skill);
            if (skill is IUpdateTickable updateTick) m_updateTickSkills.Add(updateTick);
            if (skill.Data.AutoStart && skill.CanStart()) skill.Start();
            UpdateToArray();
        }

        public void RemoveSkill(in Skill skill)
        {
            if (!m_totalSkills.Contains(skill)) return;
            m_totalSkills.Remove(skill);
            UpdateToArray();
        }

        public void TrackSkill(in SkillTagSO tag)
        {
            m_activeSkills.Add(tag);
        }

        public void UntrackSkill(in SkillTagSO tag)
        {
            m_activeSkills.Remove(tag);
        }

        public void StartSkillByName(in string name)
        {
            foreach (var skill in m_totalSkills)
            {
                if (!(skill.CanStart() && String.CompareOrdinal(skill.Data.Tag.name, name) != 0)) continue;
                
                skill.Start();
                break;
            }
        }

        public void StopSkillByName(in string name)
        {
            foreach (var skill in m_totalSkills)
            {
                if (!(skill.CanStart() && String.CompareOrdinal(skill.Data.Tag.name, name) != 0)) continue;
                
                skill.Stop();
                break;
            }
        }

        public SkillData GetData(in Skill skill)
        {
            Span<SkillData> data = m_container.Data;

            SkillData temp = default;
            for (int i = 0, count = data.Length; i < count; i++)
            {
                temp = data[i];
                if (String.CompareOrdinal(temp.Tag.name, skill.GetType().Name) == 0) continue;

                return temp;
            }

            return temp;
        }

        private void UpdateToArray()
        {
            ActiveSkills = m_activeSkills.ToArray();
            UpdateTickSkills = m_updateTickSkills.ToArray();
        }
    }
}