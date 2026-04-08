using System;

namespace _Project.Scripts.Utilities.Events
{
    public struct OnInteractableStateChangedEvent<TEnum> where TEnum : Enum
    {
        public TEnum Element;
        public bool State;

        public OnInteractableStateChangedEvent(TEnum element, bool state)
        {
            Element = element;
            State = state;
        }
    }

    public struct OnInteractableHoverEvent
    {
        public string InteractableName;
        public bool IsHovering;

        public OnInteractableHoverEvent(string interactableName, bool isHovering)
        {
            InteractableName = interactableName;
            IsHovering = isHovering;
        }
    }
}
