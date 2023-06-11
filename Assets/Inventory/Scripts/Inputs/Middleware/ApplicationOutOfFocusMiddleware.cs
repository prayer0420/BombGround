using System;
using UnityEngine;

namespace Inventory.Scripts.Inputs.Middleware
{
    [CreateAssetMenu(menuName = "Inventory/Inputs/Application Out Of Focus Middleware")]
    public class ApplicationOutOfFocusMiddleware : InputMiddleware
    {
        public override event Action OnReleaseItem;

        public override void Process(InputState inputState)
        {
            if (!Application.isFocused)
            {
                OnReleaseItem?.Invoke();
            }
        }
    }
}