using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

namespace OptimizationTool
{
    public sealed class OptimizationToolWindow : EditorWindow
    {
        #region Values
        private string _mainFolder = "Assets";

        private static readonly float _minWidth, _maxWidth = 600f;
        private static readonly float _minHeight, _maxHeight = 350f;
        #endregion

        #region Additional Classes
        private FileSearch _fileSearch;
        private DuplicatesFinder _finder;
        private AssetFinder _assetFinder;
        private ExifCleaner _exifCleaner;
        private AudioCompressor _audioCompressor;
        #endregion

        #region Other Fields
        private Vector2 _verticalScrollPosition = Vector2.zero;

        private Dictionary<string, List<string>> _duplicateFilesGroups = new();
        private List<string> _unusedAssets;
        private TMP_FontAsset _selectedFont;
        private List<string> _selectedAssets = new List<string>();

        private StateMachine _stateMachine;
        public MainState MainState { get; private set; }
        public DuplicatesFinderState DuplicatesFinderState { get; private set; }
        public UnusedAssetsHandlingState UnusedAssetsHandlingState { get; private set; }
        public ExifCleanState ExifCleanerState { get; private set; }
        public OptimizationState OptimizationState { get; private set; }
        public FontSetupState FontSetupState { get; private set; }


        private Dictionary<State, string> _states = new();
        private Dictionary<State, bool> _tasksToggle = new();
        #endregion

        #region Base Methods

        [MenuItem("Window/Tools/Optimization Tool")]
        public static void ShowWindow()
        {
            OptimizationToolWindow window = GetWindow<OptimizationToolWindow>();
            window.titleContent = new GUIContent("Optimization Tool");
            window.minSize = new Vector2(_minWidth, _minHeight);
            window.maxSize = new Vector2(_maxWidth, _maxHeight);
            window.Show();
        }

        private void Awake()
        {
            _stateMachine = new StateMachine(this);

            MainState = new MainState(_stateMachine);
            DuplicatesFinderState = new DuplicatesFinderState(_stateMachine);
            UnusedAssetsHandlingState = new UnusedAssetsHandlingState(_stateMachine);
            ExifCleanerState = new ExifCleanState(_stateMachine);
            OptimizationState = new OptimizationState(_stateMachine);
            FontSetupState = new FontSetupState(_stateMachine);

            _states.Add(DuplicatesFinderState, DuplicatesFinderState.GetButtonName());
            _states.Add(UnusedAssetsHandlingState, UnusedAssetsHandlingState.GetButtonName());
            _states.Add(ExifCleanerState, ExifCleanerState.GetButtonName());
            _states.Add(OptimizationState, OptimizationState.GetButtonName());
            _states.Add(FontSetupState, FontSetupState.GetButtonName());

            _fileSearch = new FileSearch(true, true, true, _mainFolder);
            _finder = new DuplicatesFinder();
            _audioCompressor = new AudioCompressor();

            _stateMachine.Run(MainState);
        }

        private void OnEnable()
        {
            OpenAllScenesInBuildSettings();
        }


        private void OnGUI()
        {
            _stateMachine.Update();
        }
        #endregion

        #region GUI Methods
        public void ShowMainScreen()
        {
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

        public void ShowFontSetupGUI()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Select the default font:", EditorStyles.boldLabel);
            _selectedFont = EditorGUILayout.ObjectField("TMP Font", _selectedFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;

            if (_selectedFont != null)
            {
                if (GUILayout.Button("Setup Font"))
                {
                    ApplyFont(_selectedFont);
                }
            }

            if (GUILayout.Button("Back"))
            {
                _stateMachine.SetState(MainState);
            }

            GUILayout.EndHorizontal();
        }

        public void ShowUnusedAssetsSearchGUI()
        {
            if (!Directory.Exists(_mainFolder))
            {
                EditorUtility.DisplayDialog("Error", $"Current folder '{_mainFolder}' could not be found", "Ok");
                return;
            }

            _unusedAssets?.Clear();

            try
            {
                HandleUnusedAssets();
                _stateMachine.TriggerCurrentState();
            }
            catch (System.Exception e)
            {
                Debug.Log("There are no unused assets in this project");
                _stateMachine.SetState(MainState);
            }
        }

        public void ShowDuplicatesSearchGUI()
        {
            if (!Directory.Exists(_mainFolder))
            {
                EditorUtility.DisplayDialog("Error", $"Current folder '{_mainFolder}' could not be found", "Ok");
                return;
            }

            _duplicateFilesGroups?.Clear();

            _fileSearch = new FileSearch(true, true, true, _mainFolder);

            HandleDuplicates();
            _stateMachine.TriggerCurrentState();
        }

        public void CleanMetadata()
        {
            _exifCleaner = new ExifCleaner(_mainFolder);
            ExifCleaner.OnProcessFinished += ShowMetadataRemovedNotification;
            _exifCleaner.ProcessImagesInFolder();
        }

        public void OptimizeAudio()
        {
            _audioCompressor.CompressAudio();
            _stateMachine.SetState(MainState);
        }

        private void ShowMetadataRemovedNotification()
        {
            EditorUtility.DisplayDialog("Done", $"EXIF data removed", "Ok");
            ExifCleaner.OnProcessFinished -= ShowMetadataRemovedNotification;
            _stateMachine.SetState(MainState);
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
                    SelectAllUnusedAssets();
                }

                if (GUILayout.Button("Deselect All Unused Assets"))
                {
                    _selectedAssets.Clear();
                }

                if (GUILayout.Button("Delete Selected Assets"))
                {
                    DeleteSelectedUnusedAssets();
                    if (_unusedAssets.Count == 0)
                    {
                        _stateMachine.SetState(MainState);
                    }
                    GUILayout.EndVertical();
                    return;
                }

                GUILayout.Label("Unused Assets:");
                _verticalScrollPosition = EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 2));

                foreach (var file in _unusedAssets)
                {
                    if (!File.Exists(file))
                    {
                        HandleUnusedAssets();
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
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file).GetInstanceID());
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

        public void ShowDuplicatesNavigationButtons()
        {
            try 
            {
                GUILayout.BeginVertical("box");
                if (GUILayout.Button("Delete Duplicates"))
                {
                    if (_duplicateFilesGroups != null && _duplicateFilesGroups.Count > 0)
                    {
                        _finder.RemoveDuplicates(_duplicateFilesGroups);
                    }
                }
                GUILayout.Label("Duplicate Files:");
                _verticalScrollPosition = EditorGUILayout.BeginScrollView(_verticalScrollPosition, GUILayout.Height(_maxHeight / 2));
                foreach (var duplicateGroup in _duplicateFilesGroups)
                {
                    GUILayout.Label(duplicateGroup.Key);
                    foreach (var file in duplicateGroup.Value)
                    {
                        if (!File.Exists(file))
                        {
                            HandleDuplicates();
                            continue;
                        }

                        if (GUILayout.Button(Path.GetFileName(file)))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file).GetInstanceID());
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
        #endregion

        #region Additional Methods
        private void HandleDuplicates()
        {
            var files = _fileSearch.SearchImages();
            _duplicateFilesGroups = _finder.FindDuplicates(files);
        }

        private void HandleUnusedAssets()
        {
            _assetFinder = new AssetFinder();
            _unusedAssets = _assetFinder.FindUnusedAssets();
        }

        private void ApplyFont(TMP_FontAsset font)
        {
            TMP_Text[] textComponents = FindObjectsOfType<TMP_Text>();

            if (textComponents == null || textComponents.Length == 0)
            {
                return;
            }

            foreach (TMP_Text textComponent in textComponents)
            {
                Undo.RecordObject(textComponent, "Change TMP Font");
                textComponent.font = font;
                EditorUtility.SetDirty(textComponent);
            }

            ChangeTMPSettingsFile(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(font)));

            EditorSceneManager.MarkAllScenesDirty();

            Debug.Log("Default TMP font updated to: " + font.name);
        }

        private void ChangeTMPSettingsFile(string fontAssetGuid)
        {
            TMP_Settings settings = Resources.Load<TMP_Settings>("TMP Settings");
            
            if (settings == null)
            {
                return;
            }

            var filePath = AssetDatabase.GetAssetPath(settings);

            string fileContent = File.ReadAllText(filePath);
            Debug.Log("FILE CONTENT: " + fileContent);
            string pattern = @"(m_defaultFontAsset: \{fileID: \d+, guid: )\w+(, type: \d+\})";

            string replacedText = Regex.Replace(fileContent, pattern, m =>
            {
                return m.Groups[1].Value + fontAssetGuid + m.Groups[2].Value;
            });
            Debug.Log("FILE CONTENT: " + replacedText);
            File.WriteAllText(filePath, replacedText);
            AssetDatabase.Refresh();
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

        private void SelectAllUnusedAssets()
        {   
            _selectedAssets.Clear();
            foreach (var file in _unusedAssets)
            {
                if (!File.Exists(file))
                {
                    HandleUnusedAssets();
                    continue;
                }
                _selectedAssets.Add(file);
            }
        }

        private void DeleteSelectedUnusedAssets()
        {
            foreach (var file in _selectedAssets)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    AssetDatabase.Refresh();
                }
            }
            _unusedAssets.RemoveAll(asset => _selectedAssets.Contains(asset));
            _selectedAssets.Clear();
        }
        #endregion
    }
}
