namespace OptimizationTool
{
    public class MainState : State
    {
        public MainState(StateMachine stateMachine) : base(stateMachine) { }

        public override void UpdateState()
        {
            StateMachine.Window.ShowMainScreen();
        }
    }
}