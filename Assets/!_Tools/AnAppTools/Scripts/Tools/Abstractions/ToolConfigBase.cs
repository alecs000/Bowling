using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnAppTools
{
    public abstract class ToolConfigBase : SerializedScriptableObject 
    {
        #region Constans
        protected const string NULL = nameof(NULL);
        protected const string AssetsDirectoryName = "Assets";

        protected const string SettingsTitleName = "Settings";

        protected const int ButtonSizeValue = 30;

        protected const int MinSpaceValue = 1;
        protected const int MaxSpaceValue = 5;

        protected const int MinProgressValue = 0;
        protected const int ProgressStep = 1;
        #endregion

        [PropertySpace(SpaceAfter = MaxSpaceValue, SpaceBefore = MaxSpaceValue)]
        [PropertyOrder(int.MinValue)]
        [ProgressBar(nameof(MinProgressValue), nameof(MaxProgressValue), r: 1, g: 1, b: 1, Height = ButtonSizeValue)]
        [ShowInInspector, ReadOnly, HideLabel]
        [ShowIf("@" + nameof(CurrentProgressValue) + " > " + nameof(MinProgressValue))]
        private int _currentProgressValue = MinProgressValue;
        private int _maxProgressValue = MinProgressValue;

        public bool IsExecuteMethodDisabled { get; protected set; }
        public int MaxProgressValue
        {
            get => _maxProgressValue;
            private set
            {
                _maxProgressValue = Mathf.Clamp(value, MinProgressValue, int.MaxValue);
                UnityEditor.EditorUtility.SetDirty(this);
                OnMaxProgressValueChanged?.Invoke(_maxProgressValue);
            }
        }
        public int CurrentProgressValue 
        { 
            get => _currentProgressValue;
            private set
            {
                _currentProgressValue = Mathf.Clamp(value, MinProgressValue, MaxProgressValue);
                UnityEditor.EditorUtility.SetDirty(this);
                OnCurrentProgressValueChanged?.Invoke(_currentProgressValue);
            }
        }

        [HideInInspector] public System.Action<int> OnMaxProgressValueChanged;
        [HideInInspector] public System.Action<int> OnCurrentProgressValueChanged;

        [PropertySpace(SpaceAfter = MaxSpaceValue, SpaceBefore = MaxSpaceValue)]
        [PropertyOrder(int.MinValue)]
        [Button(ButtonSizeValue)]
        [DisableIf(nameof(IsExecuteMethodDisabled))]
        [ShowIf("@" + nameof(CurrentProgressValue) + " == " + nameof(MinProgressValue))]
        public abstract void Execute();

        protected void StartProgress(int maxProgressValue)
        {
            MaxProgressValue = maxProgressValue + 2;
            CurrentProgressValue = MinProgressValue;
            AddProgress();
        }

        protected void AddProgress()
        {
            CurrentProgressValue += ProgressStep;
        }
        protected async Task AddProgress(System.Action actionBeforeAddingProgress)
        {
            await Task.Run(actionBeforeAddingProgress);
            AddProgress();
        }

        protected void CompleteProgress()
        {
            CurrentProgressValue = MinProgressValue;
            MaxProgressValue = MinProgressValue;
        }
    }
}
