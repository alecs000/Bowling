using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Android;
using UnityEngine.SceneManagement;

namespace OptimizationTool
{
    public sealed class AssetFinder
    {
        private HashSet<string> _allAssets;
        private HashSet<string> _usedAssets;

        private void Initialize()
        {
            var allScenes = GetAllScenes();
            var allAssets = AssetDatabase.FindAssets("t:texture t:material t:audioClip t:prefab t:spriteAtlas", new[] { "Assets" });
            _usedAssets = new ();
            foreach ( var asset in allScenes ) 
            {
                var dependencies = AssetDatabase.GetDependencies(asset);
                _usedAssets.UnionWith( dependencies );
            }

            _allAssets = allAssets.ToHashSet();
        }

        private List<string> GetAllScenes()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            List<string> allScenes = new ();

            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                allScenes.Add(scenePath);
            }
            return allScenes;
        }

        public List<string> FindUnusedAssets()
        {
            Initialize();

            FindUsedAssetsInIcons();

            List<string> unusedAssets = new List<string>();

            foreach (string assetGuid in _allAssets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                if (!IsInResourcesFolder(assetPath) && !_usedAssets.Contains(assetPath))
                {
                    unusedAssets.Add(assetPath);
                }
            }

            Debug.Log($"AllAssets: {_allAssets.Count}, UnusedAssets: {unusedAssets.Count}");

            return unusedAssets;
        }

        private void FindUsedAssetsInIcons()
        {
            _usedAssets.UnionWith(PlayerSettings.GetIcons(UnityEditor.Build.NamedBuildTarget.iOS, IconKind.Any).Select(icon => AssetDatabase.GetAssetPath(icon)));

            var androidIcons = new List<Texture2D>();
            androidIcons.AddRange(PlayerSettings.GetIcons(UnityEditor.Build.NamedBuildTarget.Unknown, IconKind.Any));
            androidIcons.AddRange(PlayerSettings.GetPlatformIcons(UnityEditor.Build.NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive).SelectMany(i => i.GetTextures()));
            androidIcons.AddRange(PlayerSettings.GetPlatformIcons(UnityEditor.Build.NamedBuildTarget.Android, AndroidPlatformIconKind.Round).SelectMany(i => i.GetTextures()));
            androidIcons.AddRange(PlayerSettings.GetPlatformIcons(UnityEditor.Build.NamedBuildTarget.Android, AndroidPlatformIconKind.Legacy).SelectMany(i => i.GetTextures()));

            _usedAssets.UnionWith(androidIcons.Select(icon => AssetDatabase.GetAssetPath(icon)));
        }

        private bool IsInResourcesFolder(string assetPath)
        {
            return assetPath.Split('/').Any(segment => segment == "Resources");
        }
    }
}
