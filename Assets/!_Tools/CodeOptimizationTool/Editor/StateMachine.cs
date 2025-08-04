namespace COT
{
    public class StateMachine
    {
        private OptimizationToolWindow _window;
        private State _currentState;
        public OptimizationToolWindow Window => _window;
        public StateMachine(OptimizationToolWindow window)
        {
            _window = window;
        }

        public void Run(State initialState) 
        {
            
            _currentState = initialState;
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState.UpdateState();
        }

        public void TriggerCurrentState()
        {
            _currentState.Trigger();
        }

        public void SetState(State newState) 
        {
            _currentState.Exit();
            _currentState = newState;
            _currentState.Enter();
        }
    }
}