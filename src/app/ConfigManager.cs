
using System.Runtime.InteropServices;
//using System.Reflection;
using System.Text.Json;

/// <summary>
/// Manages configuration data.
/// </summary>
public class ConfigManager
{
	/// <summary>
	/// Base path where Apple podcasts are stored
	/// </summary>
	const string DEFAULT_APPLE_PODCAST_LOCATION = "~/Library/Group Containers/243LU875E5.groups.com.apple.podcasts";

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
	/// Embedded resource key to our default config (for initial setup, resets, etc)
	/// </summary>
	const string DEFAULT_GLOBAL_CONFIG_FILE_KEY = "resources.config.json";

	/// <summary>
	/// Embedded resource key to default podcast config (for init'ing new podcasts)
	/// </summary>
	const string DEFAULT_PODCAST_CONFIG_FILE_KEY = "resources.podcast.json";

	string GlobalConfigFile => Path.Combine(GetLocalAppDataFolder, APPNAMESPACE, GLOBAL_CONFIGURATION_FILENAME);
	string PodcastRegistrations => Path.Combine(GetLocalAppDataFolder, APPNAMESPACE, PODCAST_REGISTRATIONS_FOLDER);
	string AppData => Path.Combine(GetLocalAppDataFolder, APPNAMESPACE);

	/// <summary>
	/// Checks whether our app config exists. Will create what is missing (if reasonable, otherwise throws)
	/// </summary>
	public async Task CheckAndInitGlobalConfig()
	{
		// Check:
		// 1. app folder in Application Support
		if (!Directory.Exists(AppData))
		{
			Directory.CreateDirectory(AppData);
		}

		// 2. global configuration file
		if (!File.Exists(GlobalConfigFile))
		{
			await WriteDefaultGlobalConfig(GlobalConfigFile).ConfigureAwait(false);
		}

		// 3. podcasts folder exists
		if (!Directory.Exists(PodcastRegistrations))
		{
			Directory.CreateDirectory(PodcastRegistrations);
		}

	}

	/// <summary>
	/// Resets the global config by writing the default template out.
	/// </summary>
	/// <returns></returns>
	public async Task ResetGlobalConfig()
	{
		// 2. global configuration file
		await WriteDefaultGlobalConfig(GlobalConfigFile, true).ConfigureAwait(false);
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
		var baseconfig = EmbeddedHelper.GetString("config.json");
		await File.WriteAllTextAsync(configFilePath, baseconfig).ConfigureAwait(false);
		InvalidateGlobalConfig();
	}

	async Task WriteGlobalConfig(string configFilePath, GlobalConfig newConfig)
	{
		try
		{
			if (File.Exists(configFilePath))
			{
				File.Delete(configFilePath);
			}

			_globalConfigCached = newConfig;
			var configString = JsonSerializer.Serialize(newConfig);
			await File.WriteAllTextAsync(configFilePath, configString).ConfigureAwait(false);
			InvalidateGlobalConfig();
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
		var appBase = AppData;
		if (Directory.Exists(appBase))
		{
			Directory.Delete(appBase, true);
		}
	}

	GlobalConfig? _globalConfigCached;
	bool _isGlobalConfigCacheDirty = true;

	void InvalidateGlobalConfig()
	{
		this._isGlobalConfigCacheDirty = true;
	}

	public GlobalConfig AppConfiguration
	{
		get
		{
			if (_globalConfigCached == null || _isGlobalConfigCacheDirty)
			{
				var file = File.ReadAllText(GlobalConfigFile);
				_globalConfigCached = JsonSerializer.Deserialize<GlobalConfig>(file);
				_isGlobalConfigCacheDirty = false;
			}
			return _globalConfigCached!;
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
	public void AddRegistration(string id, Registration podcast)
	{

	}

	/// <summary>
	/// Updates custom settings of a podcast.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="relative"></param>
	/// <param name="filename"></param>
	public async Task UpdateRegistation(Registration update)
	{
		var fileName = update.ConfigFilename!;
		var path = Path.Combine(PodcastRegistrations, fileName);
		var regString = JsonSerializer.Serialize(update);
		await File.WriteAllTextAsync(path, regString);
	}

	public void RemoveRegistration(string id)
	{

	}

	/// <summary>
	/// Retrieve a podcast by its unique identifier
	/// </summary>
	/// <returns></returns>
	public Registration GetRegistration(string id)
	{
		return new Registration();
	}

	public bool HasRegistration(string title, bool exact = false)
	{
		return false;
	}

	public List<Registration> FindRegistrations(string title, bool exact = false)
	{
		return new List<Registration>();
	}

	// https://stackoverflow.com/questions/45255481/alternative-for-localappdata-environment-variable-on-osx
	string GetLocalAppDataFolder => Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "~", "Library", "Application Support");

}