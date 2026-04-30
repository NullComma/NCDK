using System.Text;

namespace NCDK
{
    public static class StaticStrings
    {
        public static readonly StringBuilder Builder = new StringBuilder();
        public const string PrefixScripts = "NCDK/";
        public const string PrefixTools = "Tools/NCDK/";
        public const string DontDestroyOnLoad = "DontDestroyOnLoad";
        public const string ResourcesPath = "Assets/Resources/";
        public const string GameBundleVersion = "GameBundleVersion";
    }
}