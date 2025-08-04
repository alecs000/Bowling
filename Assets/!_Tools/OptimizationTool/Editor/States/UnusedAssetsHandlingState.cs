namespace OptimizationTool
{
    public class UnusedAssetsHandlingState : State
    {
        private const string _description = "Find and delete all unused assets in the project.";
        private const string _buttonName = "Find unused assets.";
        public UnusedAssetsHandlingState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.ShowUnusedAssetsSearchGUI();
        }

        public override void UpdateState()
        {
            if (IsTriggered)
            {
                StateMachine.Window.ShowUnusedAssetsNavigationButtons();
            }
        }
    }
}