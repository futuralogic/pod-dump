
using System.Text.Json;

namespace futura.pod_dump;
/// <summary>
/// Manages configuration data.
/// </summary>
public class ConfigManager
{

    /// <summary>
    /// Full path to global configuration file.
    /// </summary>
    /// <returns></returns>
    string GlobalConfigFilePath => Path.Combine(AppConst.Paths.AppDataPath, GLOBAL_CONFIGURATION_FILENAME);

    /// <summary>
    /// Default/global configuration file.
    /// </summary>
    const string GLOBAL_CONFIGURATION_FILENAME = "config.json";

    bool _isGlobalConfigCacheDirty = false;

    GlobalConfig? _globalConfigCached;

    /// <summary>
    /// Global application configuration
    /// </summary>
    /// <value></value>
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

    /// <summary>
    /// Checks whether our app config exists. Will create what is missing (if reasonable, otherwise throws)
    /// </summary>
    public async Task CheckAndInitGlobalConfig()
    {
        // Check:
        // 1. app folder in Application Support
        if (!Directory.Exists(AppConst.Paths.AppDataPath))
        {
            Directory.CreateDirectory(AppConst.Paths.AppDataPath);
        }

        // 2. global configuration file
        if (!File.Exists(GlobalConfigFilePath))
        {
            await WriteDefaultGlobalConfig(GlobalConfigFilePath).ConfigureAwait(false);
        }

        RegistrationManager.InitRegistrationConfig();

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

    /// <summary>
    /// Write the newConfig object to the config path.
    /// </summary>
    /// <param name="configFilePath"></param>
    /// <param name="newConfig"></param>
    /// <returns></returns>
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
        var appBase = AppConst.Paths.AppDataPath;
        if (Directory.Exists(appBase))
        {
            Directory.Delete(appBase, true);
        }
    }


    /// <summary>
    /// Invalidate the config cache.
    /// </summary>
    void InvalidateGlobalConfig()
    {
        this._isGlobalConfigCacheDirty = true;
    }

}