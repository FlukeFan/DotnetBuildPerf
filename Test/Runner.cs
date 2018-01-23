using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Test
{
    public class Runner
    {
        public void Generate()
        {
            var combinations = new Dictionary<string, RunDescription>()
            {
                { "legacy csproj (net461)",                     new RunDescription { ProjectFile = "Out_legacy.csproj",                     } },
                { "net461",                                     new RunDescription { ProjectFile = "Out_net461.csproj",                     } },
                { "net461 consuming netstandard2.0 libraries",  new RunDescription { ProjectFile = "Out_net461_consume_standard.csproj",    } },
                { "netcoreapp2.0",                              new RunDescription { ProjectFile = "Out_netcoreapp2.0.csproj",              } },
            };

            var runCount = 2;
            var outFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\Out"));

            foreach (var description in combinations.Keys)
            {
                var runDescription = combinations[description];

                for (var i = 0; i < runCount; i++)
                {
                    Time(outFolder, runDescription);
                }
            }
        }

        private void Time(string outFolder, RunDescription runDescription)
        {
            Clean(outFolder);
            DotNet(outFolder, $"restore {runDescription.ProjectFile}");

            var buildTime                   = Time(() => DotNet(outFolder, $"msbuild {runDescription.ProjectFile}"));
            var incrementalBuild_noChange   = Time(() => DotNet(outFolder, $"msbuild {runDescription.ProjectFile}"));
            Touch(Path.Combine(outFolder, @"..\Lib1\Lib1.cs"));
            var incrementalBuild_change     = Time(() => DotNet(outFolder, $"msbuild {runDescription.ProjectFile}"));
        }

        private void Touch(string file)
        {
            File.SetLastWriteTimeUtc(file, File.GetLastWriteTimeUtc(file) + TimeSpan.FromSeconds(1));
        }

        private TimeSpan Time(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
            return stopwatch.Elapsed;
        }

        private void DotNet(string outFolder, string arguments)
        {
            var exitCode = Exec.Cmd(outFolder, @"C:\Program Files\dotnet\dotnet.exe", arguments);

            if (exitCode != 0)
                throw new Exception($"Unexpected exit code: {exitCode}");
        }

        private void Clean(string outFolder)
        {
            RmDir(Path.Combine(outFolder, @"bin"));
            RmDir(Path.Combine(outFolder, @"obj"));
            RmDir(Path.Combine(outFolder, @"..\Lib2\bin"));
            RmDir(Path.Combine(outFolder, @"..\Lib2\obj"));
            RmDir(Path.Combine(outFolder, @"..\Lib1\bin"));
            RmDir(Path.Combine(outFolder, @"..\Lib1\obj"));
        }

        private void RmDir(string folder)
        {
            var until = DateTime.UtcNow + TimeSpan.FromSeconds(30);

            while (Directory.Exists(folder))
                try
                {
                    Directory.Delete(folder, true);
                }
                catch
                {
                    if (DateTime.UtcNow > until)
                        throw;
                }
        }
    }
}