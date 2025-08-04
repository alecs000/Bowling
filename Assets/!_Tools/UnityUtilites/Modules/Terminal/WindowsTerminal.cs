using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace UnityUtilites.Terminal
{
    public class WindowsTerminal
    {
        public async Task<string> RunProcessAsync(
            string programmFullName, 
            string scriptFullName) => 
            await RunProcessAsync(programmFullName, scriptFullName, string.Empty);
        public async Task<string> RunProcessAsync(
            string programmFullName, 
            string scriptFullName, 
            params string[] arguments)
        {
            var argumentsStringBuilder = new StringBuilder();
            foreach (var argument in arguments)
                argumentsStringBuilder.Append($" \"{argument}\"");
            return await RunProcessAsync(programmFullName, scriptFullName, argumentsStringBuilder.ToString());
        }
        public async Task<string> RunProcessAsync(
            string programmFullName, 
            string scriptFullName, 
            string argumentsLine)
        {
            Process process = new();
            process.StartInfo = new(programmFullName, $"\"{scriptFullName}\" {argumentsLine}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            await Task.Run(process.WaitForExit);

            return output;
        }
    }
}
