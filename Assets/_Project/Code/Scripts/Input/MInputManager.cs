using TechnoDemo.Core;
using UnityEngine;
using VContainer;

namespace TechnoDemo.Input
{
    public interface IInput
    {
        
    }
    
    [DisallowMultipleComponent]
    public sealed class MInputManager : MSceneContext, IInput
    {
        [Inject]
        private void Construct()
        {
        }
    }
}