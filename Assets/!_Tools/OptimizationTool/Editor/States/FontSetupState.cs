namespace OptimizationTool
{
    public class FontSetupState : State
    {
        private const string _description = "Setup new TextMeshPro font.";
        private const string _buttonName = "Setup font.";
        public FontSetupState(StateMachine stateMachine) : base(stateMachine)
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

        public override void UpdateState()
        {
            StateMachine.Window.ShowFontSetupGUI();
        }
    }
}