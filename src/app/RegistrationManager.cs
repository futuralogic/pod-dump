using System.Text;
using System.Text.Json;
using futura.Util;

namespace futura.pod_dump;

/// <summary>
/// Manages registrations data.
/// </summary>
public class RegistrationManager
{
    /// <summary>
    /// Directory.GetFiles search path for configuration files (JSON data)
    /// </summary>
    const string CONFIG_FILE_SEARCH_PATH = "*.json";

    /// <summary>
    /// Folder to save the collection of podcast registrations in.
    /// </summary>
    const string PODCAST_REGISTRATIONS_FOLDER = "registrations";

    /// <summary>
    /// Full path to folder that stores registrations.
    /// </summary>
    /// <returns></returns>
    static string PodcastRegistrationsPath => Path.Combine(AppConst.Paths.AppDataPath, PODCAST_REGISTRATIONS_FOLDER);

    /// <summary>
    /// Toggle to determine if our list of cached registrations is invlidated.
    /// </summary>
    bool _isRegistrationCacheDirty = false;

    /// <summary>
    /// Invalidate the registration cache when something changes.
    /// </summary>
    void InvalidateRegistrationCache()
    {
        this._isRegistrationCacheDirty = true;
    }

    bool _isRegistrationListLoaded = false;
    List<Registration> _registrations = new();

    public List<Registration> Registrations
    {
        get
        {
            if (!_isRegistrationListLoaded || _isRegistrationCacheDirty)
            {
                var files = Directory.GetFiles(PodcastRegistrationsPath, CONFIG_FILE_SEARCH_PATH);

                //Out.Line("DEBUG: Clearing out underlying list of registrations during lazy load.");
                _registrations = new List<Registration>();

                foreach (var file in files)
                {
                    var fileText = File.ReadAllText(file);
                    var reg = JsonSerializer.Deserialize<Registration>(fileText);
                    if (reg != null)
                    {
                        reg.ConfigFilename = Path.GetFileName(file);
                        if (reg.LastProcessed.HasValue)
                        {
                            reg.LastProcessedText = reg.LastProcessed.Value.RelativeTo(DateTime.Now);
                        }
                        else
                        {
                            reg.LastProcessedText = "never";
                        }
                        _registrations.Add(reg);
                    }
                }

                var pm = new PodcastManager();

                try
                {

                    // Enrich our new collection of registrations as a batch.
                    // We didn't want to enrich above in the foreach loop to avoid multiple queries into Sqlite.
                    pm.EnrichRegistrations(_registrations).Wait();
                }
                catch (Exception enrichment)
                {
                    Out.Line($"Error enriching registrations with podcast data: {enrichment.Message}");
                    throw;
                }


                _isRegistrationCacheDirty = false;
                _isRegistrationListLoaded = true;
            }

            //Out.Line("DEBUG: Returning registrations");
            return _registrations;
        }
        set
        {
            _registrations = value;
            InvalidateRegistrationCache();
        }
    }

    /// <summary>
    /// Inits the registration config (typically during first run)
    /// </summary>
    public static void InitRegistrationConfig()
    {
        // 3. podcasts folder exists
        if (!Directory.Exists(PodcastRegistrationsPath))
        {
            Directory.CreateDirectory(PodcastRegistrationsPath);
        }
    }

    public void SaveRegistrationConfigs()
    {
        foreach (var reg in _registrations)
        {
            try
            {

                var fileName = reg.ConfigFilename!;
                var path = Path.Combine(PodcastRegistrationsPath, fileName);

                // Serialize the current regisration object.
                var regString = JsonSerializer.Serialize(reg);

                // New or changed = FALSE (i.e. write file)
                // Same config = TRUE (don't update file)
                var configIsSame = false;

                if (File.Exists(path))
                {
                    var existingConfig = File.ReadAllText(path);
                    var hsh = new Hasher();
                    var hash_new = hsh.ToSha256(regString);
                    var hash_old = hsh.ToSha256(existingConfig);

                    if (hash_new == hash_old)
                    {
                        //Out.Line($"DEBUG: Config SAME for {reg.Title}");
                        configIsSame = true;
                    }
                    else
                    {
                        //Out.Line($"DEBUG: Deleting config for {reg.Title}");
                        File.Delete(path);
                    }
                }

                if (!configIsSame)
                {
                    //Out.Line($"DEBUG: Writing config for {reg.Title}");
                    File.WriteAllText(path, regString);
                }

            }
            catch (Exception saveProblem)
            {
                Out.Line($"Problem saving configs: {saveProblem.Message}");
                Out.Line(saveProblem.ToString());
                throw;
            }
        }
    }

    /// <summary>
    /// Register's a podcast.
    /// </summary>
    /// <param name="ad">Registration object</param>
    public async Task AddRegistration(Registration add)
    {
        var fileName = add.ConfigFilename!;
        var path = Path.Combine(PodcastRegistrationsPath, fileName);
        var regString = JsonSerializer.Serialize(add);
        await File.WriteAllTextAsync(path, regString);
        _registrations.Add(add);
    }

    /// <summary>
    /// Updates custom settings of a podcast. Does not persist to disk. Call SaveRegistrationConfigs() to do that.
    /// </summary>
    /// <param name="update">Registration object</param>
    public void UpdateRegistation(Registration update)
    {
        var idx = _registrations.IndexOf(update);
        //Out.Line($"DEBUG: INDEX: {idx}");
        _registrations[idx] = update;
        //Out.Line($"DEBUG: Last Processed: {_registrations[idx].LastProcessed}");
    }

    public void RemoveRegistration(string id)
    {
        var reg = GetRegistration(id);
        if (reg == null)
        {
            throw new Exception("Registration was unable to be retrieved by id.");
        }
        var path = Path.Combine(PodcastRegistrationsPath, reg.ConfigFilename);
        if (!File.Exists(path))
        {
            throw new Exception("WARNING: The podcast registration file was not found.");
        }

        File.Delete(path);
        _registrations.Remove(reg);
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