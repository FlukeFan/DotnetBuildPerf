using System.Collections.Generic;

namespace Test
{
    public class Timings
    {
        public string DotnetVersion;
        public string DotnetFileVersion;

        public IList<Timing> Frameworks = new List<Timing>();
    }
}