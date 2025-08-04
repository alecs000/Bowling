using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityUtilites.IO
{
    public class UnityDirectoryInfo
    {
        public UnityDirectoryInfo(string path) : this(new DirectoryInfo(path)) { }
        public UnityDirectoryInfo(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
            MetaFileInfo = new(directoryInfo.FullName + ".meta");
        }

        public readonly DirectoryInfo DirectoryInfo;
        public readonly FileInfo MetaFileInfo;

        public bool IsEmpty => this.GetFiles(true).Count == 0;

        public List<UnityDirectoryInfo> GetDirectories(bool recursive = false)
        {
            var result = new List<UnityDirectoryInfo>();
            var directoryInfos = recursive ? 
                DirectoryInfo.GetDirectories("*.*", SearchOption.AllDirectories) : DirectoryInfo.GetDirectories();

            foreach (var directoryInfo in directoryInfos)
                result.Add(new(directoryInfo));

            return result;
        }

        public List<UnityFileInfo> GetFiles(bool recursive = false)
        {
            var result = new List<UnityFileInfo>();
            var fileInfos = recursive ? 
                DirectoryInfo
                    .GetFiles("*.*", SearchOption.AllDirectories)
                    .Where(fileInfo => !fileInfo.Name.EndsWith(".meta")) : 
                DirectoryInfo.GetFiles("*.*").Where(fileInfo => !fileInfo.Name.EndsWith(".meta"));

            foreach (var fileInfo in fileInfos)
                result.Add(new(fileInfo));  

            return result;
        }

        public void Delete()
        {
            if (DirectoryInfo.Exists)
                DirectoryInfo.Delete(true);
            if (MetaFileInfo.Exists)
                MetaFileInfo.Delete();
        }
    }
}
