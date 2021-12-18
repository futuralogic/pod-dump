namespace futura.pod_dump;

static class AppConst
{

    /// <summary>
    /// Application namespace to use for saving config data.
    /// </summary>
    const string APPNAMESPACE = "com.futuralogic.pod-dump";

    public static class Paths
    {
        // https://stackoverflow.com/questions/45255481/alternative-for-localAppDataPath-environment-variable-on-osx
        /// <summary>
        /// Full path to user's home directory. i.e. ~ expanded
        /// </summary>
        /// <returns></returns>
        public static string HomeDirPath => Environment.GetEnvironmentVariable("HOME") ?? "";

        /// <summary>
        /// Full path to the app's data folder. (~/Library/Application Support/com.futuralogic.pod-dump)
        /// </summary>
        /// <returns></returns>
        public static string AppDataPath => Path.Combine(UserApplicationSupportPath, APPNAMESPACE);

        /// <summary>
        /// Full path to user's Application Support folder. (~/Library/Application Support)
        /// </summary>
        /// <returns></returns>
        public static string UserApplicationSupportPath => Path.Combine(HomeDirPath, "Library", "Application Support");

    }
}