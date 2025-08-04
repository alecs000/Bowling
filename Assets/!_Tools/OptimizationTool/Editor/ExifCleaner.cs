using System.IO;
using System.Linq;
using UnityEditor;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;

namespace OptimizationTool
{
    public class ExifCleaner
    {
        private string _imagesFolderPath;
        private Dictionary<string, string> _pathPairs = new Dictionary<string, string>();

        private string _tempFolderName = "!_Graphics_Temp";
        private string _tempFolderPath = "Assets/!_Graphics_Temp";

        public static event Action OnProcessFinished;

        public ExifCleaner(string imagesFolderPath)
        {
            _imagesFolderPath = imagesFolderPath;
        }

        public void ProcessImagesInFolder()
        {
            if (!Directory.Exists(_imagesFolderPath))
            {
                UnityEngine.Debug.LogError($"Images folder does not exist! Path: {_imagesFolderPath}");
                return;
            }

            string[] imageFiles = Directory.GetFiles(_imagesFolderPath, "*.*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg"))
                                    .ToArray();
            List<string> newDirectories = new List<string>(); 

            if (imageFiles.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No image files found in the specified folder.");
                return;
            }

            foreach (string imagePath in imageFiles)
            {
                string relativePath = Path.GetRelativePath(_imagesFolderPath, imagePath);

                string outputImagePath = Path.Combine(_imagesFolderPath, _tempFolderName, relativePath);

                var newDirectoryPath = Path.GetDirectoryName(outputImagePath);
                Directory.CreateDirectory(newDirectoryPath);

                newDirectories.Add(newDirectoryPath);

                RemoveExif(Path.GetFullPath(imagePath), Path.GetFullPath(outputImagePath));
                
                _pathPairs.Add(imagePath, outputImagePath);
            }

            AssetDatabase.Refresh();

            DeleteOldFiles(imageFiles);
            MoveAllNewFilesBack();

            DeleteTempDirectory(newDirectories);

            AssetDatabase.Refresh();

            OnProcessFinished?.Invoke();
        }

        private void RemoveExif(string pngFilePath, string outputFilePath)
        {
            using (Bitmap bmp = new(pngFilePath))
            {
                ImageCodecInfo pngCodec = GetEncoderInfo("image/png");
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                bmp.Save(outputFilePath, pngCodec, encoderParameters);
            }
            Console.WriteLine($"Metadata cleaned and file saved as: {outputFilePath}");
        }

        private void MoveAllNewFilesBack()
        {
            foreach (var path in _pathPairs)
            {
                File.Copy(path.Value, path.Key, true);
                AssetDatabase.DeleteAsset(path.Value);
            }
        }

        private void DeleteOldFiles(string[] files)
        {
            foreach(var file in files) 
            {
                File.Delete(file);
            }
        }

        private void DeleteTempDirectory(List<string> directories)
        {
            foreach (var directory in directories)
            {
                AssetDatabase.DeleteAsset(directory);
            }
            AssetDatabase.DeleteAsset(_tempFolderPath);
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
