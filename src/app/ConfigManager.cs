
using System.Runtime.InteropServices;
//using System.Reflection;
using System.Text.Json;

namespace futura.pod_dump;
/// <summary>
/// Manages configuration data.
/// </summary>
public class ConfigManager
{

    /// <summary>
    /// Directory.GetFiles search path for configuration files (JSON data)
    /// </summary>
    const string CONFIG_FILE_SEARCH_PATH = "*.json";

    /// <summary>
    /// Base path where Apple podcasts are stored
    /// </summary>
    const string DEFAULT_APPLE_PODCAST_LOCATION = "Library/Group Containers/243LU875E5.groups.com.apple.podcasts";

    /// <summary>
    /// Path where Apple stores its Sqlite db for podcast meta data.
    /// </summary>
    const string DEFAULT_APPLE_PODCAST_SQLITE = "Documents/MTLibrary.sqlite";

    /// <summary>
    /// Path where Apple stores the audio files once they're downloaded.
    /// </summary>
    const string DEFAULT_APPLE_PODCAST_AUDIO = "Library/Cache";

    /// <summary>
    /// Application namespace to use for saving config data.
    /// </summary>
    const string APPNAMESPACE = "com.futuralogic.pod-dump";

    /// <summary>
    /// Folder to save the collection of podcast registrations in.
    /// </summary>
    const string PODCAST_REGISTRATIONS_FOLDER = "registrations";

    /// <summary>
    /// Default/global configuration file.
    /// </summary>
    const string GLOBAL_CONFIGURATION_FILENAME = "config.json";

    /// <summary>
    /// Full path to global configuration file.
    /// </summary>
    /// <returns></returns>
    string GlobalConfigFilePath => Path.Combine(GetLocalAppDataPath, APPNAMESPACE, GLOBAL_CONFIGURATION_FILENAME);

    /// <summary>
    /// Full path to folder that stores registrations.
    /// </summary>
    /// <returns></returns>
    string PodcastRegistrationsPath => Path.Combine(GetLocalAppDataPath, APPNAMESPACE, PODCAST_REGISTRATIONS_FOLDER);

    /// <summary>
    /// Full path to the app's data folder. (~/Library/Application Support/{app namespace})
    /// </summary>
    /// <returns></returns>
    string AppDataPath => Path.Combine(GetLocalAppDataPath, APPNAMESPACE);

    // https://stackoverflow.com/questions/45255481/alternative-for-localAppDataPath-environment-variable-on-osx
    /// <summary>
    /// Full path to user's home directory. i.e. ~ expanded
    /// </summary>
    /// <returns></returns>
    string GetHomeDirPath => Environment.GetEnvironmentVariable("HOME") ?? "";

    /// <summary>
    /// Full path to user's Application Support folder. (~/Library/Application Support)
    /// </summary>
    /// <returns></returns>
    string GetLocalAppDataPath => Path.Combine(GetHomeDirPath, "Library", "Application Support");

    /// <summary>
    /// Full path to Apple Podcasts Sqlite database where podcast and episode metadata is stored.
    /// </summary>
    /// <returns></returns>
    public string ApplePodcastSqlitePath => Path.Combine(GetHomeDirPath, DEFAULT_APPLE_PODCAST_LOCATION, DEFAULT_APPLE_PODCAST_SQLITE);

    /// <summary>
    /// Full path to Apple Podcasts audio (mp3, etc) files.
    /// </summary>
    /// <returns></returns>
    public string ApplePodcastAudioFilePath => Path.Combine(GetHomeDirPath, DEFAULT_APPLE_PODCAST_LOCATION, DEFAULT_APPLE_PODCAST_AUDIO);

    /// <summary>
    /// Checks whether our app config exists. Will create what is missing (if reasonable, otherwise throws)
    /// </summary>
    public async Task CheckAndInitGlobalConfig()
    {
        // Check:
        // 1. app folder in Application Support
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }

        // 2. global configuration file
        if (!File.Exists(GlobalConfigFilePath))
        {
            await WriteDefaultGlobalConfig(GlobalConfigFilePath).ConfigureAwait(false);
        }

        // 3. podcasts folder exists
        if (!Directory.Exists(PodcastRegistrationsPath))
        {
            Directory.CreateDirectory(PodcastRegistrationsPath);
        }

    }

    /// <summary>
    /// Resets the global config by writing the default template out.
    /// </summary>
    /// <returns></returns>
    public async Task ResetGlobalConfig()
    {
        // 2. global configuration file
        await WriteDefaultGlobalConfig(GlobalConfigFilePath, true).ConfigureAwait(false);
    }


    /// <summary>
    /// Creates a global config file.
    /// </summary>
    /// <param name="configFilePath"></param>
    /// <param name="overwrite"></param>
    /// <returns></returns>
    async Task WriteDefaultGlobalConfig(string configFilePath, bool overwrite = false)
    {
        if (overwrite && File.Exists(configFilePath))
        {
            File.Delete(configFilePath);
        }
        var baseconfig = new GlobalConfig();
        await WriteGlobalConfig(configFilePath, baseconfig);
    }

    async Task WriteGlobalConfig(string configFilePath, GlobalConfig newConfig)
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                File.Delete(configFilePath);
            }

            var configString = JsonSerializer.Serialize(newConfig);
            await File.WriteAllTextAsync(configFilePath, configString).ConfigureAwait(false);
            AppConfiguration = newConfig;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error writing configuration: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Removes the entire app folder from Application Support. No UI surface at this time.
    /// </summary>
    public void CleanupApp()
    {
        var appBase = AppDataPath;
        if (Directory.Exists(appBase))
        {
            Directory.Delete(appBase, true);
        }
    }

    bool _isGlobalConfigCacheDirty = false;

    void InvalidateGlobalConfig()
    {
        this._isGlobalConfigCacheDirty = true;
    }

    GlobalConfig? _globalConfigCached;
    public GlobalConfig AppConfiguration
    {
        get
        {
            if (_globalConfigCached == null || _isGlobalConfigCacheDirty)
            {
                var file = File.ReadAllText(GlobalConfigFilePath);
                _globalConfigCached = JsonSerializer.Deserialize<GlobalConfig>(file);
                _isGlobalConfigCacheDirty = false;
            }
            return _globalConfigCached!;
        }
        protected set
        {
            _globalConfigCached = value;
            InvalidateGlobalConfig();
        }
    }

    bool _isRegistrationCacheDirty = false;

    void InvalidateRegistrationCache()
    {
        this._isRegistrationCacheDirty = true;
    }

    bool _isRegistrationListLoaded = false;
    List<Registration> _registrations = new List<Registration>();

    public List<Registration> Registrations
    {
        get
        {
            if (!_isRegistrationListLoaded || _isRegistrationCacheDirty)
            {
                var files = Directory.GetFiles(PodcastRegistrationsPath, CONFIG_FILE_SEARCH_PATH);

                _registrations = new List<Registration>();

                var pm = new PodcastManager();

                foreach (var file in files)
                {
                    var fileText = File.ReadAllText(file);
                    var reg = JsonSerializer.Deserialize<Registration>(fileText);
                    if (reg != null)
                    {
                        reg.ConfigFilename = file;
                        _registrations.Add(reg);
                    }
                }

                // Enrich our new collection of registrations as a batch.
                // We didn't want to enrich above in the foreach loop to avoid multiple queries into Sqlite.
                pm.EnrichRegistrations(_registrations).Wait();

                _isRegistrationCacheDirty = false;
                _isRegistrationListLoaded = true;
            }
            return _registrations;
        }
        set
        {
            _registrations = value;
            InvalidateRegistrationCache();
        }
    }

    /// <summary>
    /// Register's a podcast.
    /// </summary>
    /// <param name="id">Unique ID that associates us back to Apple's db</param>
    /// <param name="title">Friendly title from podcast</param>
    /// <param name="target">Custom target (if overridden), otherwise null</param>
    /// <param name="relative">Custom relative sub-path (if overridden), otherwise null</param>
    /// <param name="filename">Custom filenaming convention (if overridden), otherwise null</param>
    public async Task AddRegistration(Registration add)
    {
        var fileName = add.ConfigFilename!;
        var path = Path.Combine(PodcastRegistrationsPath, fileName);
        var regString = JsonSerializer.Serialize(add);
        await File.WriteAllTextAsync(path, regString);
        InvalidateRegistrationCache();
    }

    /// <summary>
    /// Updates custom settings of a podcast.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="relative"></param>
    /// <param name="filename"></param>
    public async Task UpdateRegistation(Registration update)
    {
        RemoveRegistration(update.UniqueId);
        await AddRegistration(update);
    }

    public void RemoveRegistration(string id)
    {
        var reg = GetRegistration(id);
        if (reg == null)
        {
            throw new Exception("Registration was unable to be retrieved by id.");
        }
        if (!File.Exists(reg.ConfigFilename))
        {
            throw new Exception("WARNING: The podcast registration file was not found.");
        }

        File.Delete(reg.ConfigFilename);

        InvalidateRegistrationCache();
    }

    /// <summary>
    /// Retrieve a podcast by its unique identifier
    /// </summary>
    /// <returns></returns>
    public Registration? GetRegistration(string id)
    {
        var found = from reg in Registrations
                    where reg.UniqueId.StartsWith(id, StringComparison.OrdinalIgnoreCase)
                    select reg;

        return found.FirstOrDefault();
    }

    /// <summary>
    /// Returns whether the registration exists by unique id.
    /// </summary>
    /// <param name="id">Unique SHA256 hash of the registration.</param>
    /// <returns></returns>
    public bool HasRegistration(string id)
    {
        var found = from reg in Registrations
                    where reg.UniqueId.StartsWith(id, StringComparison.OrdinalIgnoreCase)
                    select reg;
        return found.Any();
    }

    /// <summary>
    /// Returns whether the registration exists by searching for it.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="exact"></param>
    /// <returns></returns>
    public bool HasRegistration(string title, bool exact)
    {
        var found = from reg in Registrations
                    where
                    (
                        (exact && reg.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) ||
                        (!exact && reg.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                    )
                    select reg;

        return found.Any();
    }

    public IEnumerable<Registration> FindRegistrations(string title, bool exact = false)
    {
        var searchTerm = title.ToLower();

        return from reg in Registrations
               where
               !string.IsNullOrEmpty(reg.Title) &&
               (
                   (!exact && reg.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (exact && reg.Title.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
               select reg;
    }


}