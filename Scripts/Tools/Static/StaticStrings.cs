using System.Text;

namespace NullCore
{
    public static class StaticStrings
    {
        public static readonly StringBuilder Builder = new StringBuilder();
        public const string PrefixScripts = "NullCore/";
        public const string PrefixTools = "Tools/NullCore/";
        public const string DontDestroyOnLoad = "DontDestroyOnLoad";
        public const string ResourcesPath = "Assets/Resources/";
        public const string GameBundleVersion = "GameBundleVersion";
    }
}