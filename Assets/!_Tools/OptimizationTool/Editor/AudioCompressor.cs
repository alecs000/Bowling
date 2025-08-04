using UnityEngine;
using UnityEditor;
using System.IO;

namespace OptimizationTool
{
    public class AudioCompressor
    {
        public void CompressAudio()
        {
            var allAudioAssets = AssetDatabase.FindAssets("t:audioClip", new[] { "Assets" });
            
            foreach (var asset in allAudioAssets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(asset);
                AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;
                if (importer == null) { continue; }
                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                settings.quality = 0.25f;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
        }
    }
}