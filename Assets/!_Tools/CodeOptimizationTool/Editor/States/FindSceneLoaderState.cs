namespace COT
{
    public class FindSceneLoaderState : State
    {
        private const string _description = "Find All Object with SceneLoader";
        private const string _buttonName = "Find SceneLoader";
        public FindSceneLoaderState(StateMachine stateMachine) : base(stateMachine)
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
                StateMachine.Window.ShowFindNavigation(AlgorithmState.FindSceneLoader);
            }
        }
    }
}