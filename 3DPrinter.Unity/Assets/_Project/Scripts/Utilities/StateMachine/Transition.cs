using System;

namespace Game.Scripts.Utilities.StateMachine
{
    public class Transition<TState> where TState : Enum
    {
        public TState From { get; private set; }
        public TState To { get; private set; }
        public Func<bool> Condition { get; private set; }
        public bool IsGlobal { get; private set; }

        public Transition(TState from, TState to, Func<bool> condition)
        {
            From = from;
            To = to;
            Condition = condition;
            IsGlobal = false;
        }

        public Transition(TState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
            IsGlobal = true;
        }
    }
}