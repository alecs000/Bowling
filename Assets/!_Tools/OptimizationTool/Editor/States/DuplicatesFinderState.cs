namespace OptimizationTool
{
    public class DuplicatesFinderState : State
    {
        private const string _description = "Find and delete duplicates in the project.";
        private const string _buttonName = "Find duplicates.";
        public DuplicatesFinderState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.ShowDuplicatesSearchGUI();
        }

        public override void UpdateState()
        {
            if (IsTriggered)
            {
                StateMachine.Window.ShowDuplicatesNavigationButtons();
            }
        }
    }
}