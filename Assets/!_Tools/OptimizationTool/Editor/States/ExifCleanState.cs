namespace OptimizationTool
{
    public class ExifCleanState : State
    {
        private const string _description = "Delete all metadata from images.";
        private const string _buttonName = "Delete metadata.";
        public ExifCleanState(StateMachine stateMachine) : base(stateMachine)
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
            StateMachine.Window.CleanMetadata();
        }
    }
}