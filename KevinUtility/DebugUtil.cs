using System.Diagnostics;

namespace KevinUtility
{
    public static class DebugUtil
    {
        public static void Log(string format, params string[] args)
        {
#if DEBUG
           Debug.WriteLine(format, args);
#endif
        }

        public static void Log(string value)
        {
#if DEBUG
            Debug.WriteLine(value);
#endif
        }

        public static void Log(object value)
        {
#if DEBUG
            Debug.WriteLine(value);
#endif
        }
    }
}
