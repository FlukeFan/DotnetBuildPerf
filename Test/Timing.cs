using System;

namespace Test
{
    public class Timing
    {
        public string Description;

        public TimeSpan Build;
        public TimeSpan IncrementalBuildWithoutChange;
        public TimeSpan IncrementalBuildWithChange;
    }
}