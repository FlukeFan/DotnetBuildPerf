using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner();
            var timings = runner.Generate();

            ReportToConsole(timings);
        }

        private static void ReportToConsole(Timings timings)
        {
            Console.WriteLine($"\n\n\n");
            Console.WriteLine($"Timing Results");
            Console.WriteLine($"==============\n");
            Console.WriteLine($"dotnet.exe --version: {timings.DotnetVersion}");
            Console.WriteLine($"dotnet.exe: {timings.DotnetFileVersion}\n");
            Console.WriteLine("Description                Build       Incremental  Incremental  ");
            Console.WriteLine("                                       (with change)(no changes) ");
            Console.WriteLine("-----------------------------------------------------------------");

            foreach (var timing in timings.Frameworks)
            {
                Console.WriteLine($"{timing.Description.PadRight(25)} {Seconds(timing.Build).PadRight(12)} {Seconds(timing.IncrementalBuildWithChange).PadRight(12)} {Seconds(timing.IncrementalBuildWithoutChange).PadRight(12)}");
            }

            Console.WriteLine("\nMultiplier\n");

            long? baselineBuild = null;
            long? baselineIncrementalWithoutChange = null;
            long? baselineIncrementalWithChange = null;

            foreach (var timing in timings.Frameworks)
            {
                var buildTicks = timing.Build.Ticks;
                var incrementalWithoutChangeTicks = timing.IncrementalBuildWithoutChange.Ticks;
                var incrementalWithChangeTicks = timing.IncrementalBuildWithChange.Ticks;

                baselineBuild = baselineBuild ?? buildTicks;
                baselineIncrementalWithoutChange = baselineIncrementalWithoutChange ?? incrementalWithoutChangeTicks;
                baselineIncrementalWithChange = baselineIncrementalWithChange ?? incrementalWithChangeTicks;

                var buildMultiplier = buildTicks / (double)baselineBuild.Value;
                var incrementalWithoutChangeMultiplier = incrementalWithoutChangeTicks / (double)baselineIncrementalWithoutChange;
                var incrementalWithChangeMultiplier = incrementalWithChangeTicks / (double)baselineIncrementalWithChange;

                Console.WriteLine($"{timing.Description.PadRight(25)} {Multiplier(buildMultiplier).PadRight(12)} {Multiplier(incrementalWithChangeMultiplier).PadRight(12)} {Multiplier(incrementalWithoutChangeMultiplier).PadRight(12)}");
            }
        }

        private static string Seconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalSeconds.ToString("0.00s");
        }

        private static string Multiplier(double value)
        {
            return value.ToString("0.0");
        }
    }
}
