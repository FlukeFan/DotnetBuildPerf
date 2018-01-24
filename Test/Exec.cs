using System.Diagnostics;

namespace Test
{
    public static class Exec
    {
        public static int Cmd(string workingDir, string program, string args)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = workingDir;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        public static string GetOutput(string workingDir, string program, string args)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = workingDir;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
        }
    }
}
