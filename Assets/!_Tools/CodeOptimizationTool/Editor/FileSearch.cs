using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace COT
{
    public class FileSearch
    {
       
        private string _graphicsFolder;

        public FileSearch(string path)
        {
            _graphicsFolder = path;
        }

        public IEnumerable<string> SearchFile()
        {
            var formats = new List<string>();
            formats.Add("cs");
            var files = Directory
                .GetFiles(_graphicsFolder, "*.*", SearchOption.AllDirectories)
                .Where(f => formats.Contains(Path.GetExtension(f).TrimStart('.').ToLowerInvariant()));

            return files;
        }
        public  IEnumerable<string> SearchInAsset()
        {
            var formats = new List<string>();
            formats.Add("asset");
            var files = Directory
                .GetFiles(_graphicsFolder, "*.*", SearchOption.AllDirectories)
                .Where(f => formats.Contains(Path.GetExtension(f).TrimStart('.').ToLowerInvariant()));

            return files;
        }
        public  IEnumerable<string> SearchInScene()
        {
            var formats = new List<string>();
            formats.Add("unity");
            var files = Directory
                .GetFiles(_graphicsFolder, "*.*", SearchOption.AllDirectories)
                .Where(f => formats.Contains(Path.GetExtension(f).TrimStart('.').ToLowerInvariant()));

            return files;
        }
        public  IEnumerable<string> SearchASMDEF(string directory)
        {
            var formats = new List<string>();
            formats.Add("asmdef");
            var files = Directory
                .GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(f => formats.Contains(Path.GetExtension(f).TrimStart('.').ToLowerInvariant()));
            
            return files;
        }

        public string GetPath()
        {
            return _graphicsFolder;
        }
    }
}