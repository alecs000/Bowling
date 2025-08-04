namespace COT
{
    public class DisbandDirectoryToolConfigState : State
    {
        private const string _description = "Disband All Tool in Directory";
        private const string _buttonName = "Disband Directory";
        public DisbandDirectoryToolConfigState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.DisbandDirectoryToolConfig();
        }
    }
}

