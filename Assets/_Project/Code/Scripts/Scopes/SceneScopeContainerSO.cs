using NaughtyAttributes;
using TechnoDemo.Actions;
using TechnoDemo.Core;
using TechnoDemo.Player;
using UnityEngine;

namespace TechnoDemo.Scopes
{
    [CreateAssetMenu(fileName = nameof(SceneScopeContainerSO), menuName = "SO/Scopes/Scene Container")]
    public sealed class SceneScopeContainerSO : ScriptableObject
    {
        [Expandable] public GameManagerSO GameManagerSo;
        [Expandable] public PlayerSettingsSO PlayerSettingsSo;
        [Expandable] public ActionContainerSO ActionContainerSo;
    }
}