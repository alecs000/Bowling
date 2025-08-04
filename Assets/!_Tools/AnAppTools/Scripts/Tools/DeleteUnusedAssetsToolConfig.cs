using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilites.IO;

namespace AnAppTools
{
    public class DeleteUnusedAssetsToolConfig : ToolConfigBase
    {
        #region Settings
        [Title(SettingsTitleName)]

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath]
        public List<string> IncludedDirectoryPaths = new()
        {
            "Assets/!_App"
        };
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        [FolderPath]
        public List<string> ExcludedDirectoryPaths = new()
        {
            "Assets/!_App/Resources",
            "Assets/!_App/Scripts",
        };
        #endregion

        public override async void Execute()
        {
            try
            {
                var allResourcesDirs = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories);

                foreach (var dir in allResourcesDirs)
                    if (!ExcludedDirectoryPaths.Contains(dir))
                        ExcludedDirectoryPaths.Add(dir);

                ExcludedDirectoryPaths = ExcludedDirectoryPaths.Distinct().ToList();

                var excludedDirectoryInfos = ExcludedDirectoryPaths.Select(edp => new DirectoryInfo(edp));
                var includedDirectoryInfos = IncludedDirectoryPaths.Select(idp => new DirectoryInfo(idp));
                var dependentUnityFileInfos = new HashSet<UnityFileInfo>(); 

                void AddToDependentUnityFileInfos(UnityFileInfo unityFileInfo)
                {
                    var fullName = unityFileInfo.FileInfo.FullName;
                    var canAdd = 
                        !excludedDirectoryInfos.Any(edi => fullName.StartsWith(edi.FullName)) &&
                        includedDirectoryInfos.Any(idi => fullName.StartsWith(idi.FullName));

                    if (canAdd)
                        dependentUnityFileInfos.Add(unityFileInfo);
                }

                #region Assets from scenes in BuildSettings
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                    foreach (var assetPath in AssetDatabase.GetDependencies(SceneUtility.GetScenePathByBuildIndex(i)))
                        AddToDependentUnityFileInfos(new(assetPath));
                #endregion
                
                #region Assets from PlayerSettings
                foreach (var icon in PlayerSettings.GetIcons(NamedBuildTarget.Unknown, IconKind.Any))
                    if (icon != null)
                        AddToDependentUnityFileInfos(new(icon));
                        
                foreach (var splashScreenLogo in PlayerSettings.SplashScreen.logos)
                    if (splashScreenLogo.logo != null)
                        AddToDependentUnityFileInfos(new(splashScreenLogo.logo));

                if (PlayerSettings.SplashScreen.background != null)
                        AddToDependentUnityFileInfos(new(PlayerSettings.SplashScreen.background));
                #endregion

                var unityFileInfos = new List<UnityFileInfo>();
                IncludedDirectoryPaths.ForEach(idp => new UnityDirectoryInfo(idp)
                    .GetDirectories(true)
                    .Where(udi => !excludedDirectoryInfos
                        .Any(edp => udi.DirectoryInfo.FullName.StartsWith(edp.FullName)))
                    .ToList()
                    .ForEach(udi => unityFileInfos.AddRange(udi.GetFiles())));

                StartProgress(unityFileInfos.Count * 2);

                var unusedUnityFileInfos = new List<UnityFileInfo>();
                foreach (var unityFileInfo in unityFileInfos)
                {
                    await AddProgress(() =>
                    {
                        var fullName = unityFileInfo.FileInfo.FullName;
                        if (!dependentUnityFileInfos.Any(dufi => fullName == dufi.FileInfo.FullName))
                            unusedUnityFileInfos.Add(unityFileInfo);
                    });
                }

                var dialogResult = EditorUtility.DisplayDialogComplex(
                    "Delete unused assets", 
                    $"Delete {unusedUnityFileInfos.Count} assets?",
                    "Show assets",
                    "Delete", 
                    "Cancel");
                switch (dialogResult)
                {
                    case 0:
                        Debug.Log(new string('-', 100));
                        foreach (var unusedUnityFileInfo in unusedUnityFileInfos)
                            await AddProgress(() => Debug.Log($" * {unusedUnityFileInfo.FileInfo.FullName}"));
                        Debug.Log(new string('-', 100));
                        break;
                    case 1:
                        foreach (var unusedUnityFileInfo in unusedUnityFileInfos)
                            await AddProgress(() => unusedUnityFileInfo.Delete());
                        break;
                }

                AssetDatabase.Refresh();
            }
            catch
            {
                throw new System.NotImplementedException();
            }
            finally
            {
                CompleteProgress();
            }

        }
    }
}
