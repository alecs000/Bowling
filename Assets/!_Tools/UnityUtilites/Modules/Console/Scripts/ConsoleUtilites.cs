using UnityEditor;

namespace UnityUtilites.Console
{
    public static class ConsoleUtilites 
    {
        public static void ClearConsole() => System.Reflection.Assembly
            .GetAssembly(typeof(SceneView))
            .GetType("UnityEditor.LogEntries")
            .GetMethod("Clear")
            .Invoke(new(), null);
    }
}
