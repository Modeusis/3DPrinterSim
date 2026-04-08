using System;
using _Project.Scripts.Player.Interaction;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Interactables
{
    public abstract class InteractableBase<TEnum> : MonoBehaviour, IClickable where TEnum : Enum
    {
        [SerializeField] private string _interactableName;
        [SerializeField] private Animator _animator;
        
        private EventBus _eventBus;
        private int _cachedAnimatorHash;

        protected abstract string AnimatorParameterName { get; }
        protected abstract TEnum StateElement { get; }
        
        protected bool CurrentState;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected virtual void Awake()
        {
            _cachedAnimatorHash = Animator.StringToHash(AnimatorParameterName);
        }

        public void OnBeginHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent(_interactableName, true));
        }

        public void OnEndHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent(_interactableName, false));
        }

        public virtual void OnClick()
        {
            ToggleState();
        }

        private void ToggleState()
        {
            CurrentState = !CurrentState;
            
            _animator.SetBool(_cachedAnimatorHash, CurrentState);
            
            _eventBus.Publish(new OnInteractableStateChangedEvent<TEnum>(StateElement, CurrentState));
        }

        protected void SetState(bool state)
        {
            if (CurrentState == state)
                return;

            CurrentState = state;
            _animator.SetBool(_cachedAnimatorHash, CurrentState);
            _eventBus.Publish(new OnInteractableStateChangedEvent<TEnum>(StateElement, CurrentState));
        }
    }
}
