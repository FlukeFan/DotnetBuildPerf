namespace DotnetPerf
{
    public static class Lib2
    {
        public static string Value()
        {
            return $"Lib2 value using ({Lib1.Value("Lib2")})";
        }
    }
}
