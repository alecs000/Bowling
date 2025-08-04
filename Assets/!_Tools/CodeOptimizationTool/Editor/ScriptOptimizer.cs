using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace COT
{
    public class ScriptOptimizer
    {
       

        public List<string> GetNotOptimizedFiles(IEnumerable<string> files,AlgorithmState algorithmState)
        {
            var filesToOptimize = new List<string>();

            
            foreach (var filePath in files)
            {
                string text = File.ReadAllText(filePath);
                

                switch (algorithmState)
                {
                    case AlgorithmState.FindGameObjectFind:
                        if (Regex.IsMatch(text, "GameObject[.]Find[(]"))
                        {
                            filesToOptimize.Add(filePath);
                        }
                        break;
                    case AlgorithmState.FindLinks:
                       if (Regex.IsMatch(text,"https"))
                        {
                            filesToOptimize.Add(filePath);
                        }
                        break;
                    case AlgorithmState.FindSceneLoader:
                        if (Regex.IsMatch(text,"SceneManager[.]LoadScene[(][\"]"))
                        {
                            
                            filesToOptimize.Add(filePath);
                        }
                        break;
                    case AlgorithmState.FindGameByType:
                         if (Regex.IsMatch(text,"FindObjectOfType"))
                        {
                            filesToOptimize.Add(filePath);
                        }
                        break;  
                    case AlgorithmState.FindIAPLegacy:
                        if (Regex.IsMatch(text,"component: {fileID: 353088492}"))
                        {
                            filesToOptimize.Add(filePath);
                        }
                        break; 
                    default:
                        Debug.Log("Nothing");
                        break;
                }
            }
            return filesToOptimize;
        }
      
    }
}
