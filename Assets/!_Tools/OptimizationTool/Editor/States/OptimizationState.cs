namespace OptimizationTool
{
    public class OptimizationState : State
    {
        private const string _description = "Optimize audio in the project.";
        private const string _buttonName = "Optimize audio.";
        public OptimizationState(StateMachine stateMachine) : base(stateMachine)
        {
        }

        public override string GetDescription()
        {
            return _description;
        }

        public override string GetButtonName()
        {
            return _buttonName;
        }

        public override void Enter()
        {
            StateMachine.Window.OptimizeAudio();
        }
    }
}