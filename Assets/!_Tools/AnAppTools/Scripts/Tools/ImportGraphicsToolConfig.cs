using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityUtilites.IO;
using UnityUtilites.Terminal;

namespace AnAppTools
{
    public class ImportGraphicsToolConfig : ToolConfigBase
    {
        #region Settings
        [Title(SettingsTitleName)]

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MaxSpaceValue)]
        [ToggleLeft]
        public bool CanShowLogs = false;

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        public string ImportedImageName = "Image_{0}";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MinSpaceValue)]
        public string ImportedFontName = "Font_{0}";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        public string ImportedAudioName = "Audio_{0}";

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        public HashSet<string> AllowedImageExtensions = new()
        {
            ".png",
            ".jpg",
        };
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MinSpaceValue)]
        public HashSet<string> AllowedFontExtensions = new()
        {
            ".ttf",
        };
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        public HashSet<string> AllowedZipExtensions = new()
        {
            ".zip",
        };
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        public HashSet<string> AllowedAudioExtensions = new()
        {
            ".mp3",
            ".wav",
        };

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath(AbsolutePath = true)]
        public string PythonFullName = "C:/Program Files/Python311/python.exe";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        [Sirenix.OdinInspector.FilePath]
        public string AutocropImagesPythonScriptPath = "Assets/Plugins/AnAppTools/Scripts/Python/AutocropImages.py";

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MaxSpaceValue)]
        [ToggleLeft]
        public bool CanFindImportedGraphicsDirectoryByProjectName = false;

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath]
        public string SpritesDirectoryPath = "Assets/!_App/Graphics/Sprites";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath]
        public string FontsDirectoryPath = "Assets/!_App/Graphics/Fonts";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath]
        public string AudioDirectoryPath = "Assets/!_App/Graphics/Audio";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        [FolderPath(AbsolutePath = true)]
        [HideIf(nameof(CanFindImportedGraphicsDirectoryByProjectName))]
        public string ImportedGraphicsDirectoryPath = $"C:/Users/{NULL}/Downloads/{NULL}";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        [FolderPath(AbsolutePath = true)]
        [ShowIf(nameof(CanFindImportedGraphicsDirectoryByProjectName))]
        public string ImportedGraphicsParentDirectoryPath = $"C:/Users/{NULL}/Downloads";
        #endregion

        public override async void Execute()
        {
            try
            {
                UnityDirectoryInfo CreateUnityDirectoryInfo(string directoryPath)
                {
                    var unityDirectoryInfo = new UnityDirectoryInfo(directoryPath);
                    if (!unityDirectoryInfo.DirectoryInfo.Exists)
                        throw new DirectoryNotFoundException(unityDirectoryInfo.DirectoryInfo.FullName);
                    return unityDirectoryInfo;
                }
                
                var spritesUnityDirectoryInfo = CreateUnityDirectoryInfo(SpritesDirectoryPath);
                var fontsUnityDirectoryInfo = CreateUnityDirectoryInfo(FontsDirectoryPath);
                var audioUnityDirectoryInfo = CreateUnityDirectoryInfo(AudioDirectoryPath);

                DirectoryInfo importedGraphicsDirectoryInfo;
                if (CanFindImportedGraphicsDirectoryByProjectName)
                {
                    var projectName = Directory.GetParent(Application.dataPath).Name;
                    var importedGraphicsDirectoryPath = Path.Combine(ImportedGraphicsParentDirectoryPath, projectName);

                    importedGraphicsDirectoryInfo = new DirectoryInfo(
                        !Directory.Exists(importedGraphicsDirectoryPath) ?
                            EditorUtility.OpenFolderPanel("Directory", "", "") : importedGraphicsDirectoryPath);
                }
                else
                {
                    importedGraphicsDirectoryInfo = new DirectoryInfo(
                        !Directory.Exists(ImportedGraphicsDirectoryPath) ?
                            EditorUtility.OpenFolderPanel("Directory", "", "") : ImportedGraphicsDirectoryPath);
                }

                var imageFileInfos = new List<FileInfo>();
                var fontFileInfos = new List<FileInfo>();
                var audioFileInfos = new List<FileInfo>();

                foreach (var unityFileInfo in new UnityDirectoryInfo(importedGraphicsDirectoryInfo).GetFiles(true))
                {
                    var fileInfo = unityFileInfo.FileInfo;
                    if (AllowedZipExtensions.Any(aze => fileInfo.Name.EndsWith(aze)))
                    {
                        var destinationUnityDirectoryInfo = new UnityDirectoryInfo(Path.Combine(
                            fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name)));
                        System.IO.Compression.ZipFile.ExtractToDirectory(
                            fileInfo.FullName, destinationUnityDirectoryInfo.DirectoryInfo.FullName);
                        if (CanShowLogs)
                            Debug.Log(
                                $"\'{fileInfo.FullName}\' unziped to " + 
                                $"\'{destinationUnityDirectoryInfo.DirectoryInfo.FullName}\'");

                        fileInfo.Delete();
                        if (CanShowLogs)
                            Debug.Log($"Deleted \'{fileInfo.FullName}\'");
                    }
                }

                foreach (var unityFileInfo in new UnityDirectoryInfo(importedGraphicsDirectoryInfo).GetFiles(true))
                {
                    var fileInfo = unityFileInfo.FileInfo;
                    if (AllowedImageExtensions.Any(aie => fileInfo.Name.EndsWith(aie)))
                        imageFileInfos.Add(fileInfo);
                    else if (AllowedFontExtensions.Any(afe => fileInfo.Name.EndsWith(afe)))
                        fontFileInfos.Add(fileInfo);
                    else if (AllowedAudioExtensions.Any(aae => fileInfo.Name.EndsWith(aae)))
                        audioFileInfos.Add(fileInfo);
                }

                var spriteUnityFileInfos = spritesUnityDirectoryInfo.GetFiles();
                var fontUnityFileInfos = fontsUnityDirectoryInfo.GetFiles();
                var audioUnityFileInfos = audioUnityDirectoryInfo.GetFiles();

                StartProgress(
                    spriteUnityFileInfos.Count + 
                    fontUnityFileInfos.Count +
                    audioUnityFileInfos.Count +
                    imageFileInfos.Count + 1 + 
                    fontFileInfos.Count +
                    audioFileInfos.Count);

                async Task ClearAndCopyFiles(
                    List<UnityFileInfo> unityFileInfos,
                    List<FileInfo> fileInfos,
                    UnityDirectoryInfo unityDirectoryInfo,
                    string name)
                {
                    foreach (var unityFileInfo in unityFileInfos)
                        await AddProgress(() => 
                        {
                            unityFileInfo.Delete();
                            if (CanShowLogs)
                                Debug.Log($"Deleted \'{unityFileInfo.FileInfo.FullName}\'");
                        });

                    for (int i = 0; i < fileInfos.Count; i++)
                        await AddProgress(() =>
                        {
                            var fileInfo = fileInfos[i];
                            var newFullName = Path.Combine(
                                unityDirectoryInfo.DirectoryInfo.FullName, 
                                string.Format(name, i) + fileInfo.Extension);

                            fileInfo.CopyTo(newFullName);
                            if (CanShowLogs)
                                Debug.Log($"\'{fileInfo.FullName}\' copied to \'{newFullName}\'");
                        });
                }

                await ClearAndCopyFiles(
                    spriteUnityFileInfos, imageFileInfos, spritesUnityDirectoryInfo, ImportedImageName);
                await ClearAndCopyFiles(
                    fontUnityFileInfos, fontFileInfos, fontsUnityDirectoryInfo, ImportedFontName);
                await ClearAndCopyFiles(
                    audioUnityFileInfos, audioFileInfos, audioUnityDirectoryInfo, ImportedAudioName);

                var spritesFullNames = new List<string>();
                spritesUnityDirectoryInfo.GetFiles().ForEach(ufi => spritesFullNames.Add(ufi.FileInfo.FullName));
                await new WindowsTerminal().RunProcessAsync(
                    PythonFullName, new FileInfo(AutocropImagesPythonScriptPath).FullName, spritesFullNames.ToArray());

                AddProgress();

                AssetDatabase.Refresh();
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            finally
            {
                CompleteProgress();
            }
        }

        private bool CheckAlloweExtensions(List<string> allowedExtensions)
        {
            void ResetDirecotryName(int index) => allowedExtensions[index] = NULL;

            for (int i = 0; i < allowedExtensions.Count; i++)
                if (allowedExtensions[i] == string.Empty)
                    ResetDirecotryName(i);
                else
                    for (int j = 0; j < allowedExtensions.Count; j++)
                    {
                        var canReset = i != j && 
                            allowedExtensions[i] == allowedExtensions[j] && 
                            allowedExtensions[i] != NULL;
                        if (canReset)
                            ResetDirecotryName(j);
                    }

            return true;
        }
    }
}
