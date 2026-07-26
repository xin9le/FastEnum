#if NETFRAMEWORK
#pragma warning disable IDE0130
#pragma warning disable IDE0161



namespace System
{
    internal static class EnumExtensions
    {
        extension(Enum)
        {
            public static string GetName<T>(T value)
                => Enum.GetName(typeof(T), value);


            public static string[] GetNames<T>()
                => (string[])Enum.GetNames(typeof(T));


            public static T[] GetValues<T>()
                => (T[])Enum.GetValues(typeof(T));


            public static T Parse<T>(string? name)
                => (T)Enum.Parse(typeof(T), name);
        }
    }
}
#endif
