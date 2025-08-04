namespace COT
{
    public class GameObjectFinderState : State
    {
        private const string _description = "Find All Object with GameObjectFind ";
        private const string _buttonName = "Find GameObjectFind";
        public GameObjectFinderState(StateMachine stateMachine) : base(stateMachine)
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
                StateMachine.Window.ShowFindNavigation(AlgorithmState.FindGameObjectFind);
            }
        }
    }
}