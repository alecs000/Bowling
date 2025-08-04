using COT;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


public class FindAllIAPBUttonLegacy : State
{
    private const string _description = "Find all IAPButtonsLegacy in project";
    private const string _buttonName = "Find IAPButtonsLegacy.";
    public FindAllIAPBUttonLegacy(StateMachine stateMachine) : base(stateMachine)
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
        OpenAllScenesInBuildSettings();
        StateMachine.Window.ShowDuplicatesSearchGUI();
    }

    public override void UpdateState()
    {
        if (IsTriggered)
        {
          
            StateMachine.Window.ShowFindNavigation(AlgorithmState.FindIAPLegacy);
        }
    }
    private void OpenAllScenesInBuildSettings()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene currentScene = EditorSceneManager.GetSceneAt(i);
            EditorSceneManager.CloseScene(currentScene, true);
        }

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }
    }
}

