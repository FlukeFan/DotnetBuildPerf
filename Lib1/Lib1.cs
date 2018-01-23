namespace DotnetBuildPerf
{
    public static class Lib1
    {
        public static string Value(string parentLib)
        {
            return $"Lib1 value from parent {parentLib}";
        }
    }
}
