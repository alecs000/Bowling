namespace COT
{
    public class DeleteUnusedAssetsToolConfigState : State
    {
        private const string _description = "Delete All Unused Assets from project";
        private const string _buttonName = "Delete Unused Assets";
        public DeleteUnusedAssetsToolConfigState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.DeleteUnusedAssetsToolConfig();
        }
    }  
}

