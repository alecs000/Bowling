namespace COT
{
    public class LinksFinderState : State
    {
        private const string _description = "Find all Links in the project";
        private const string _buttonName = "Find Links.";
        public LinksFinderState(StateMachine stateMachine) : base(stateMachine)
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
                StateMachine.Window.ShowFindNavigation(AlgorithmState.FindLinks);
            }
        }
    }
}