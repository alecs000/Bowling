using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityUtilites.IO;

namespace AnAppTools
{
    public class DisbandDirectoryToolConfig : ToolConfigBase
    {
        #region Settings
        [Title(SettingsTitleName)]

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MaxSpaceValue)]
        [ToggleLeft]
        public bool CanShowLogs = false;

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        [FolderPath]
        public string DisbandedDirectoryPath = "Assets/!_MoveToAppFolder";

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MaxSpaceValue)]
        public Dictionary<string, HashSet<string>> RelatedDirectoryNames = new()
        {
            { "Assets/!_App/Scripts", new() { 
                "Script", 
                "Scripts", } },
            { "Assets/!_App/Animations", new() { 
                "Anim", 
                "Anims", 
                "Animation", 
                "Animations",
                "Timeline",
                "Timelines", } },
            { "Assets/!_App/Materials", new() { 
                "Material", 
                "Materials", 
                "Matherial", 
                "Matherials", } },
            { "Assets/!_App/Models", new() { 
                "Model", 
                "Models", 
                "Mesh", 
                "Meshs", } },
            { "Assets/!_App/Prefabs", new() { 
                "Prefab", 
                "Prefabs", } },
            { "Assets/!_App/Shaders", new() { 
                "Shader", 
                "Shaders", } },
            { "Assets/!_App/Scenes", new() { 
                "Scene", 
                "Scenes", } },
            { "Assets/!_App/Resources", new() { 
                "Resources", } },
            { "Assets/!_App/Graphics/Sprites", new() { 
                "Sprite", 
                "Sprites", 
                "Texture", 
                "Textures", 
                "Art", 
                "Arts", 
                "Img", 
                "Imgs", 
                "Image", 
                "Images", } },
            { "Assets/!_App/Graphics/Fonts", new() { 
                "Font", 
                "Fonts", } },
            { "Assets/!_App/Graphics/Audio", new() { 
                "Sound", 
                "Sounds", 
                "Music", 
                "Musics", 
                "Audio",
                "AudioClip",
                "AudioClips",
                "AudioMixer",
                "AudioMixers", } },
        };

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        public string DisbandedParentDirectoryName = "_Game";
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        public string DisbandedDirectoryName = "{0}_{1}";
        #endregion
        
        public override async void Execute()
        {
            try
            {
                var disbandedUnityDirectoryInfo = new UnityDirectoryInfo(DisbandedDirectoryPath);
                if (!disbandedUnityDirectoryInfo.DirectoryInfo.Exists)
                    throw new System.IO.DirectoryNotFoundException(disbandedUnityDirectoryInfo.DirectoryInfo.FullName);
                var disbandedUnityDirectoryInfos = disbandedUnityDirectoryInfo.GetDirectories(true);
                
                StartProgress(disbandedUnityDirectoryInfos.Count);

                foreach (var keyValuePair in RelatedDirectoryNames)
                {
                    var destinationDirectoryInfo = new DirectoryInfo(
                        Path.Combine(keyValuePair.Key, DisbandedParentDirectoryName));
                    if (!destinationDirectoryInfo.Exists)
                    {
                        destinationDirectoryInfo.Create();
                        Debug.Log($"Created \'{destinationDirectoryInfo.FullName}\'");
                    }

                    foreach (var value in keyValuePair.Value)
                    {
                        var destinationDirectoryName = value.ToLower();
                        var index = 0;

                        for (int i = 0; i < disbandedUnityDirectoryInfos.Count; i++)
                        {
                            var sourceDirectoryInfo = disbandedUnityDirectoryInfos[i].DirectoryInfo;
                            if (!disbandedUnityDirectoryInfos[i].IsEmpty && 
                                sourceDirectoryInfo.Name.ToLower() == destinationDirectoryName)
                            {
                                await AddProgress(() =>
                                {
                                    var newDestinationDirectoryInfo = new DirectoryInfo(Path.Combine(
                                        destinationDirectoryInfo.FullName, 
                                        string.Format(DisbandedDirectoryName, value, index++)));

                                    sourceDirectoryInfo.MoveTo(newDestinationDirectoryInfo.FullName);
                                    Debug.Log($"\'{sourceDirectoryInfo.FullName}\' moved to \'{newDestinationDirectoryInfo.FullName}\'");

                                    disbandedUnityDirectoryInfos.RemoveAt(i);
                                });
                            }
                        }                
                    }
                }

                if (disbandedUnityDirectoryInfo.IsEmpty)
                    disbandedUnityDirectoryInfo.Delete();

                AssetDatabase.Refresh();
            }
            catch (System.Exception exception)
            {
                AssetDatabase.Refresh();
                throw exception;
            }
            finally
            {
                CompleteProgress();
            }
        }
    }
}
