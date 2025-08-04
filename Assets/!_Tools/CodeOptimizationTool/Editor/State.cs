namespace COT
{
    public abstract class State
    {
        private StateMachine _stateMachine;
        private bool _isTriggered;

        protected bool IsTriggered => _isTriggered;
        protected StateMachine StateMachine => _stateMachine;

        public State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual string GetButtonName() 
        {
            return string.Empty;
        }

        public virtual string GetDescription()
        {
            return string.Empty;
        }

        public virtual void Enter() { }
        public virtual void Trigger() 
        {
            _isTriggered = true;
        }
        public virtual void UpdateState() { }
        public virtual void Exit() 
        {
            _isTriggered = false;
        }
    }
}