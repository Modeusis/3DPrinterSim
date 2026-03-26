using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Utilities.StateMachine
{
    public class FSM<TState> where TState : System.Enum
    {
        private Dictionary<TState, State<TState>> _states;
        private List<Transition<TState>> _transitions;
        private State<TState> _currentState;

        public FSM(Dictionary<TState, State<TState>> states, List<Transition<TState>> transitions, TState startingState)
        {
            _states = states;
            _transitions = transitions;
            ChangeState(startingState);
        }
        
        public void Update()
        {
            _currentState?.Update();
        }

        public void LateUpdate()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                var transition = _transitions[i];
                
                bool fromMatches = transition.IsGlobal || EqualityComparer<TState>.Default.Equals(transition.From, _currentState.StateType);
                bool conditionMet = transition.Condition();
                
                if (fromMatches && conditionMet && 
                    !EqualityComparer<TState>.Default.Equals(_currentState.StateType, transition.To))
                {
                    ChangeState(transition.To);
                    return;
                }
            }
        }
        
        private void ChangeState(TState newState)
        {
            if (!_states.TryGetValue(newState, out State<TState> state))
            {
                Debug.LogWarning($"Cannot change FSM state to {newState}!");
                return;
            }
            
            _currentState?.Exit();
            _currentState = state;
            _currentState?.Enter();
        }
    }
}