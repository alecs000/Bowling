using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace COT
{
    public class RemovePlayerPrefsDeleteAllState : State
    {
        private const string _description = "Removes all calls to PlayerPrefs.DeleteAll()";
        private const string _buttonName = "Remove PlayerPrefs.DeleteAll";

        public RemovePlayerPrefsDeleteAllState(StateMachine stateMachine) : base(stateMachine) { }

        public override string GetDescription()
        {
            return _description;
        }

        public override string GetButtonName()
        {
            return _buttonName;
        }

        public override void Enter()
        {
            int removedCount = 0;

            string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            string thisFilePath = GetThisFilePath();

            foreach (var file in csFiles)
            {
                if (file == thisFilePath)
                    continue;

                string content = File.ReadAllText(file);
                string pattern = @"PlayerPrefs\.DeleteAll\s*\(\s*\)\s*;";
                if (Regex.IsMatch(content, pattern))
                {
                    string newContent = Regex.Replace(content, pattern, "", RegexOptions.Multiline);
                    File.WriteAllText(file, newContent);
                    removedCount++;
                    Debug.Log($"[Remove DeleteAll] Deleted in: {file}");
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done", $"PlayerPrefs.DeleteAll() calls removed: {removedCount}", "OK");

            StateMachine.Window.ShowMainScreen();
        }

        private string GetThisFilePath()
        {
            string[] matches = Directory.GetFiles(Application.dataPath, "RemovePlayerPrefsDeleteAllState.cs", SearchOption.AllDirectories);
            return matches.Length > 0 ? matches[0] : string.Empty;
        }
    }
}
