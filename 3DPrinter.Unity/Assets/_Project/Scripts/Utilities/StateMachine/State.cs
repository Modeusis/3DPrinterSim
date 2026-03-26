namespace Game.Scripts.Utilities.StateMachine
{
    public abstract class State<TState> where TState : System.Enum
    {
        public TState StateType { get; protected set; }
        
        public abstract void Enter();
        
        public abstract void Update();
        
        public abstract void Exit();
    }
}