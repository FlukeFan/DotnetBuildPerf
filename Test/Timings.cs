using System.Collections.Generic;

namespace Test
{
    public class Timings
    {
        public string DotnetVersion;
        public string DotnetFileVersion;
        public string SdkVersion;
        public string SdkBuildVersion;

        public IList<Timing> Frameworks = new List<Timing>();
    }
}