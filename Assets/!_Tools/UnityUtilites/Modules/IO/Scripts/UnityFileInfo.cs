using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtilites.IO
{
    public class UnityFileInfo
    {
        public UnityFileInfo(Object asset) : this(AssetDatabase.GetAssetPath(asset)) { }
        public UnityFileInfo(string path) : this(new FileInfo(path)) { }
        public UnityFileInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            MetaFileInfo = new(fileInfo.FullName + ".meta");
        }

        public readonly FileInfo FileInfo;
        public readonly FileInfo MetaFileInfo;

        public void Delete()
        {
            if (FileInfo.Exists)
                FileInfo.Delete();
            if (MetaFileInfo.Exists)
                MetaFileInfo.Delete();
        }
    }
}
