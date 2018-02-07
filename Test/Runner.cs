using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Test
{
    public class Runner
    {
        public const string DotnetExe = @"C:\Program Files\dotnet\dotnet.exe";

        public Timings Generate()
        {
            var combinations = new Dictionary<string, RunDescription>()
            {
                { "legacy csproj (net461)",     new RunDescription { ProjectFile = "Out_legacy.csproj",                     } },
                { "net461",                     new RunDescription { ProjectFile = "Out_net461.csproj",                     } },
                { "netstandard2.0 libraries",   new RunDescription { ProjectFile = "Out_net461_consume_standard.csproj",    } },
                { "netcoreapp2.0",              new RunDescription { ProjectFile = "Out_netcoreapp2.0.csproj",              } },
            };

            var timings = new Timings
            {
                DotnetVersion = DotnetVersion(),
                DotnetFileVersion = DotnetFileVersion(),
            };

            DotnetSdkVersion(timings);

            var runCount = 5;
            var outFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\Out"));
            var totalRuns = runCount * combinations.Keys.Count;
            var currentRun = 0;

            foreach (var description in combinations.Keys)
            {
                var timing = new Timing
                {
                    Description = description,
                };

                var runDescription = combinations[description];
                var runTimes = new List<Timing>();

                for (var i = 0; i < runCount; i++)
                {
                    currentRun++;
                    Console.WriteLine($"\n\nRun {currentRun} of {totalRuns} {description} run {i + 1} of {runCount}\n");

                    var runTime = Time(outFolder, runDescription);
                    runTimes.Add(runTime);
                }

                timing.Build = TimeSpan.FromTicks(runTimes.Sum(rt => rt.Build.Ticks) / runTimes.Count);
                timing.IncrementalBuildWithoutChange = TimeSpan.FromTicks(runTimes.Sum(rt => rt.IncrementalBuildWithoutChange.Ticks) / runTimes.Count);
                timing.IncrementalBuildWithChange = TimeSpan.FromTicks(runTimes.Sum(rt => rt.IncrementalBuildWithChange.Ticks) / runTimes.Count);
                timings.Frameworks.Add(timing);
            }

            return timings;
        }

        private Timing Time(string outFolder, RunDescription runDescription)
        {
            Clean(outFolder);
            Dotnet(outFolder, $"restore {runDescription.ProjectFile}");

            Console.WriteLine($"Performing inital build (after restore)");
            var buildTime                   = Time(() => Dotnet(outFolder, $"msbuild {runDescription.ProjectFile}"));

            Console.WriteLine($"Performing incremental build after a file has changed");
            Touch(Path.Combine(outFolder, @"..\Lib1\Lib1.cs"));
            var incrementalBuild_change     = Time(() => Dotnet(outFolder, $"msbuild {runDescription.ProjectFile}"));

            Console.WriteLine($"Performing incremental build when nothing has changed");
            var incrementalBuild_noChange   = Time(() => Dotnet(outFolder, $"msbuild {runDescription.ProjectFile}"));

            return new Timing
            {
                Build = buildTime,
                IncrementalBuildWithoutChange = incrementalBuild_noChange,
                IncrementalBuildWithChange = incrementalBuild_change,
            };
        }

        private void Touch(string file)
        {
            File.SetLastWriteTimeUtc(file, DateTime.UtcNow + TimeSpan.FromSeconds(1));
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

        private void Dotnet(string outFolder, string arguments)
        {
            var exitCode = Exec.Cmd(outFolder, DotnetExe, arguments);

            if (exitCode != 0)
                throw new Exception($"Unexpected exit code: {exitCode}");
        }

        private string DotnetFileVersion()
        {
            return FileVersionInfo.GetVersionInfo(DotnetExe).FileVersion;
        }

        private string DotnetVersion()
        {
            var version = Exec.GetOutput(Path.GetDirectoryName(DotnetExe), DotnetExe, "--version");
            return version.Replace("\n", "").Replace("\r", "");
        }

        private void DotnetSdkVersion(Timings timings)
        {
            var info = Exec.GetOutput(Path.GetDirectoryName(DotnetExe), DotnetExe, "--info");

            var lastLines = info.Split("\n")
                .Where(l => l.Trim().Length > 0)
                .TakeLast(2)
                .ToList();

            if (lastLines.Count < 1)
                return;

            timings.SdkVersion = lastLines[0];

            if (lastLines.Count < 2)
                return;

            timings.SdkBuildVersion = lastLines[1];
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