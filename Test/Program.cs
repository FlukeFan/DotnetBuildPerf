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
            Console.WriteLine($"==============\n\n");
            Console.WriteLine($"dotnet.exe: {timings.DotnetVersion}\n");
            Console.WriteLine("Description                     Build          Incremental     Incremental     ");
            Console.WriteLine("                                               (no changes)    (with change)   ");
            Console.WriteLine("-------------------------------------------------------------------------------");

            foreach (var timing in timings.Frameworks)
            {
                Console.WriteLine($"{timing.Description.PadRight(31)} {Seconds(timing.Build).PadRight(15)} {Seconds(timing.IncrementalBuildWithoutChange).PadRight(15)} {Seconds(timing.IncrementalBuildWithChange).PadRight(15)}");
            }

            Console.WriteLine("\nPercentages\n");

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

                var buildIncrease = (buildTicks - baselineBuild.Value) / (double)baselineBuild.Value;
                var incrementalWithoutChangeIncrease = (incrementalWithoutChangeTicks - baselineIncrementalWithoutChange.Value) / (double)baselineIncrementalWithoutChange;
                var incrementalWithChangeIncrease = (incrementalWithChangeTicks - baselineIncrementalWithChange.Value) / (double)baselineIncrementalWithChange;

                Console.WriteLine($"{timing.Description.PadRight(31)} {Percent(buildIncrease).PadRight(15)} {Percent(incrementalWithoutChangeIncrease).PadRight(15)} {Percent(incrementalWithChangeIncrease).PadRight(15)}");
            }
        }

        private static string Seconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalSeconds.ToString("0.00s");
        }

        private static string Percent(double value)
        {
            return value.ToString("+000%");
        }
    }
}
