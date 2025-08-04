namespace COT
{
    public class SynchronizePackagesToolConfigState : State
    {
        private const string _description = "Synchronize Packages in project";
        private const string _buttonName = "Synchronize Packages";
        public SynchronizePackagesToolConfigState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.SynchronizePackagesToolConfig();
        }
    }
    
}

 

