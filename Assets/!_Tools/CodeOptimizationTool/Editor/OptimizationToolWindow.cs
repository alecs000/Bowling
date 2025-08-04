using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;
using AnAppTools;
using Sirenix.Utilities;

#if UNITY_IOS
using UnityEngine.Purchasing;
#endif


namespace COT
{
    public enum AlgorithmState
    {
        FindGameObjectFind,
        FindLinks,
        FindGameByType,
        FindSceneLoader,
        FindIAPLegacy,
    }

    public sealed class OptimizationToolWindow : EditorWindow
    {
        #region Values

        private string _mainFolder;

        private static readonly float _minWidth, _maxWidth = 600f;
        private static readonly float _minHeight, _maxHeight = 350f;

        #endregion

        #region Additional Classes

        private FileSearch _fileSearchApp;
        private ScriptOptimizer _scriptOptimizer;

        #endregion

        #region Other Fields

        private Vector2 _verticalScrollPosition = Vector2.zero;

        private Dictionary<string, List<string>> _duplicateFilesGroups = new();
        private List<string> _unusedAssets;
        private TMP_FontAsset _selectedFont;
        private List<string> _selectedAssets = new List<string>();

        private StateMachine _stateMachine;
        public MainState MainState { get; private set; }
        public LinksFinderState DuplicatesFinderState { get; private set; }
        public FindSceneLoaderState UnusedAssetsHandlingState { get; private set; }
        public GameObjectFinderState ExifCleanerState { get; private set; }
        public FindByTypeState OptimizationState { get; private set; }
        public RemovePlayerPrefsDeleteAllState RemoveDeleteAll { get; private set; }

        public DeleteUnusedAssetsToolConfigState DeleteUnused { get; private set; }
        public DisbandDirectoryToolConfigState DisbandDirectory { get; private set; }
        public SynchronizePackagesToolConfigState Synchronize { get; private set; }
        public FindAllIAPBUttonLegacy IAllIapbUttonLegacy { get; private set; }


        private Dictionary<State, string> _states = new();
        private Dictionary<State, bool> _tasksToggle = new();
        private DisbandDirectoryToolConfig directoryToolConfig;
        private SynchronizePackagesToolConfig synchronizePackagesTool;
        private DeleteUnusedAssetsToolConfig deleteUnused;

        #endregion

        #region Base Methods

        [MenuItem("Window/Tools/Code Optimization Tool")]
        public static void ShowWindow()
        {
            OptimizationToolWindow window = GetWindow<OptimizationToolWindow>();
            window.titleContent = new GUIContent("Code Optimization Tool");
            window.minSize = new Vector2(_minWidth, _minHeight);
            window.maxSize = new Vector2(_maxWidth, _maxHeight);
            window.Show();
        }

        private void Awake()
        {
            _stateMachine = new StateMachine(this);
            foreach (var file in Directory.GetDirectories("Assets"))
            {
                if (Regex.IsMatch(file, "!_App"))
                {
                    _mainFolder = file;
                }
            }


            MainState = new MainState(_stateMachine);
            DuplicatesFinderState = new LinksFinderState(_stateMachine);
            UnusedAssetsHandlingState = new FindSceneLoaderState(_stateMachine);
            ExifCleanerState = new GameObjectFinderState(_stateMachine);
            OptimizationState = new FindByTypeState(_stateMachine);
            DeleteUnused = new DeleteUnusedAssetsToolConfigState(_stateMachine);
            DisbandDirectory = new DisbandDirectoryToolConfigState(_stateMachine);
            Synchronize = new SynchronizePackagesToolConfigState(_stateMachine);
            IAllIapbUttonLegacy = new FindAllIAPBUttonLegacy(_stateMachine);
            RemoveDeleteAll = new RemovePlayerPrefsDeleteAllState(_stateMachine);

            _states.Add(DuplicatesFinderState, DuplicatesFinderState.GetButtonName());
            _states.Add(UnusedAssetsHandlingState, UnusedAssetsHandlingState.GetButtonName());
            _states.Add(ExifCleanerState, ExifCleanerState.GetButtonName());
            _states.Add(OptimizationState, OptimizationState.GetButtonName());
            _states.Add(DeleteUnused, DeleteUnused.GetButtonName());
            _states.Add(DisbandDirectory, DisbandDirectory.GetButtonName());
            _states.Add(Synchronize, Synchronize.GetButtonName());
            _states.Add(IAllIapbUttonLegacy, IAllIapbUttonLegacy.GetButtonName());
            _states.Add(RemoveDeleteAll, RemoveDeleteAll.GetButtonName());

            _fileSearchApp = new FileSearch(_mainFolder);
            _scriptOptimizer = new ScriptOptimizer();

            _stateMachine.Run(MainState);
        }


        private void OnGUI()
        {
            _stateMachine.Update();
        }

        #endregion

        #region GUI Methods

        private void ShowErrorWindow()
        {
            GUILayout.BeginVertical();
            _verticalScrollPosition =
                EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 1.2f));
            EditorGUILayout.HelpBox("App folder not found", MessageType.Info);
        }

        public void ShowMainScreen()
        {
            if (_mainFolder.IsNullOrWhitespace())
            {
                ShowErrorWindow();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                return;
            }
            
            GUILayout.BeginVertical();
            _verticalScrollPosition = EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 1.2f));

            int index = 1;
            foreach (var state in _states)
            {
                var currentKey = state.Key;
                EditorGUILayout.HelpBox($"{index}. {currentKey.GetDescription()}", MessageType.Info);
                index++;
                GUILayout.BeginHorizontal();
                bool selectedValue = false;

                if (_tasksToggle.ContainsKey(currentKey))
                {
                    _tasksToggle.TryGetValue(currentKey, out selectedValue);
                }

                bool isSelected = selectedValue;
                bool newSelection = GUILayout.Toggle(isSelected, "", GUILayout.Width(20));

                if (newSelection && !isSelected)
                {
                    _tasksToggle.Add(currentKey, true);
                }
                else if (!newSelection && selectedValue)
                {
                    _tasksToggle.Add(currentKey, false);
                }

                if (GUILayout.Button(state.Value))
                {
                    _tasksToggle.Add(currentKey, true);
                    _stateMachine.SetState(currentKey);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }


        public void ShowDuplicatesSearchGUI()
        {
            if (!Directory.Exists(_mainFolder))
            {
                EditorUtility.DisplayDialog("Error", $"Current folder '{_mainFolder}' could not be found", "Ok");
                return;
            }

            _duplicateFilesGroups?.Clear();


            _stateMachine.TriggerCurrentState();
        }

        #endregion

        #region Navigation Methods

        public void ShowUnusedAssetsNavigationButtons()
        {
            try
            {
                GUILayout.BeginVertical("box");

                if (GUILayout.Button("Back"))
                {
                    _stateMachine.SetState(MainState);
                }

                if (GUILayout.Button("Select All Unused Assets"))
                {
                }

                if (GUILayout.Button("Deselect All Unused Assets"))
                {
                    _selectedAssets.Clear();
                }

                if (GUILayout.Button("Delete Selected Assets"))
                {
                    if (_unusedAssets.Count == 0)
                    {
                        _stateMachine.SetState(MainState);
                    }

                    GUILayout.EndVertical();
                    return;
                }

                GUILayout.Label("Unused Assets:");
                _verticalScrollPosition =
                    EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 2));

                foreach (var file in _unusedAssets)
                {
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    GUILayout.BeginHorizontal();

                    bool isSelected = _selectedAssets.Contains(file);
                    bool newSelection = GUILayout.Toggle(isSelected, "", GUILayout.Width(20));

                    if (newSelection && !isSelected)
                    {
                        _selectedAssets.Add(file);
                    }
                    else if (!newSelection && isSelected)
                    {
                        _selectedAssets.Remove(file);
                    }

                    if (GUILayout.Button(Path.GetFileName(file)))
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)
                            .GetInstanceID());
                    }

                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            catch (System.Exception e)
            {
                Debug.Log("There are no unused assets in this project");
                _stateMachine.SetState(MainState);
                GUILayout.Label("There are no unused assets in this project");
            }
        }

        public void ShowFindNavigation(AlgorithmState func)
        {
            try
            {
                GUILayout.BeginVertical("box");
                if (func == AlgorithmState.FindIAPLegacy)
                {
                    GUILayout.Label("Object name with IAPButton Legacy:");
                }
                else
                {
                    GUILayout.Label("Scripts:");
                }
               
                _verticalScrollPosition =
                    EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 2));
                if (func != AlgorithmState.FindLinks && func != AlgorithmState.FindIAPLegacy)
                {
                    foreach (var file in _scriptOptimizer.GetNotOptimizedFiles(_fileSearchApp.SearchFile(),
                                 func))
                    {
                        if (GUILayout.Button(Path.GetFileName(file)))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)
                                .GetInstanceID());
                        }
                    }
                }
                else if (func == AlgorithmState.FindIAPLegacy)
                {
                #if UNITY_IOS
                    IAPButton[]  rootObjectsInScene = FindObjectsOfType<IAPButton>();
                    foreach (var file in rootObjectsInScene)
                    {
                        GUILayout.Button(file.name);
                    }
                #endif
                }
                else
                {
                    foreach (var file in _scriptOptimizer.GetNotOptimizedFiles(_fileSearchApp.SearchFile(),
                                 func))
                    {
                        if (!file.Equals($"{_fileSearchApp.GetPath()}\\Resources\\AppSettings.asset"))
                        {
                            if (GUILayout.Button(Path.GetFileName(file)))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)
                                    .GetInstanceID());
                            }
                        }
                    }

                    foreach (var file in _scriptOptimizer.GetNotOptimizedFiles(_fileSearchApp.SearchInAsset(),
                                 func))
                    {
                        if (!file.Equals($"{_fileSearchApp.GetPath()}\\Resources\\AppSettings.asset"))
                        {
                            if (GUILayout.Button(Path.GetFileName(file)))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)
                                    .GetInstanceID());
                            }
                        }
                    }

                    foreach (var file in _scriptOptimizer.GetNotOptimizedFiles(_fileSearchApp.SearchInScene(),
                                 func))
                    {
                        if (!file.Equals($"{_fileSearchApp.GetPath()}\\Resources\\AppSettings.asset"))
                        {
                            if (GUILayout.Button(Path.GetFileName(file)))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)
                                    .GetInstanceID());
                            }
                        }
                    }
                }


                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Back"))
                {
                    _stateMachine.SetState(MainState);
                }

                GUILayout.EndVertical();
            }
            catch (System.Exception e)
            {
                Debug.Log("There are no duplicates in this project");
                GUILayout.Label("There are no duplicates in this project");
                _stateMachine.SetState(MainState);
            }
        }
       

        public void DisbandDirectoryToolConfig()
        {
            directoryToolConfig.Execute();
        }

        public void SynchronizePackagesToolConfig()
        {
            synchronizePackagesTool.Execute();
        }

        public void DeleteUnusedAssetsToolConfig()
        {
            deleteUnused.Execute();
        }

        #endregion

    }
}