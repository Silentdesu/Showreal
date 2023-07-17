using System;
using System.Collections.Generic;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;
using UnityEngine;
using VContainer;

namespace TechnoDemo.Actions
{
    public interface IActionHandler
    {
        IPlayer Player { get; }
        ActionTagSO[] ActiveActions { get; }
        IUpdateTickable[] UpdateTickActions { get; }
        
        Action<BaseAction> onActionStartedEvent { get; }
        Action<BaseAction> onActionStoppedEvent { get; }

        void AddSkill(in BaseAction baseAction);
        void RemoveAction(in BaseAction baseAction);
        void TrackAction(in ActionTagSO tag);
        void UntrackAction(in ActionTagSO tag);
        void StartActionByName(in string name);
        void StopActionByName(in string name);
        ActionData GetData(in BaseAction baseAction);
        T GetAction<T>() where T : BaseAction;
        bool TryGetAction<T>(out T action) where T : BaseAction;
    }

    public sealed class ActionHandler : IActionHandler
    {
        public IPlayer Player { get; private set; }
        public ActionTagSO[] ActiveActions { get; private set; }
        public IUpdateTickable[] UpdateTickActions { get; private set; }

        public Action<BaseAction> onActionStartedEvent { get; set; }
        public Action<BaseAction> onActionStoppedEvent { get; set; }

        private ActionContainerSO m_container;
        private BaseAction[] m_actions;
        
        private readonly List<BaseAction> m_totalActions = new List<BaseAction>(10);
        private readonly List<ActionTagSO> m_activeActions = new List<ActionTagSO>(3);
        private readonly List<IUpdateTickable> m_updateTickActions = new List<IUpdateTickable>(3);

        [Inject]
        public ActionHandler(ActionContainerSO container)
        {
            m_container = container;
        }
        
        public void AddSkill(in BaseAction baseAction)
        {
            if (m_totalActions.Contains(baseAction)) return;
            m_totalActions.Add(baseAction);
            if (baseAction is IUpdateTickable updateTick) m_updateTickActions.Add(updateTick);
            if (baseAction.Data.AutoStart && baseAction.CanStart()) baseAction.Start();
            UpdateToArray();
        }

        public void RemoveAction(in BaseAction baseAction)
        {
            if (!m_totalActions.Contains(baseAction)) return;
            m_totalActions.Remove(baseAction);
            UpdateToArray();
        }

        public void TrackAction(in ActionTagSO tag)
        {
            m_activeActions.Add(tag);
        }

        public void UntrackAction(in ActionTagSO tag)
        {
            m_activeActions.Remove(tag);
        }

        public void StartActionByName(in string name)
        {
            foreach (var skill in m_totalActions)
            {
                if (!(skill.CanStart() && String.CompareOrdinal(skill.Data.Tag.name, name) != 0)) continue;
                
                skill.Start();
                break;
            }
        }

        public void StopActionByName(in string name)
        {
            foreach (var skill in m_totalActions)
            {
                if (!(skill.CanStart() && String.CompareOrdinal(skill.Data.Tag.name, name) != 0)) continue;
                
                skill.Stop();
                break;
            }
        }

        public ActionData GetData(in BaseAction baseAction)
        {
            Span<ActionData> data = m_container.Data;

            ActionData temp = default;
            for (int i = 0, count = data.Length; i < count; i++)
            {
                temp = data[i];
                if (String.CompareOrdinal(temp.Tag.name, baseAction.GetType().Name) != 0) continue;

                return temp;
            }

            return temp;
        }

        public T GetAction<T>() where T : BaseAction
        {
            Span<BaseAction> actions = m_actions;
            
            for (int i = 0, count = actions.Length; i < count; i++)
            {
                if (actions[i] is T action) return action;
            }

            return null;
        }

        public bool TryGetAction<T>(out T action) where T : BaseAction
        {
            action = GetAction<T>();
            return action != null;
        }

        private void UpdateToArray()
        {
            ActiveActions = m_activeActions.ToArray();
            UpdateTickActions = m_updateTickActions.ToArray();
            m_actions = m_totalActions.ToArray();
        }
    }

    public static partial class AnimatorParameters
    {
        public static int ActionID = Animator.StringToHash("ActionID");
    }
}