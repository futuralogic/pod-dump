using System.Text.Json;
using System.Text.Json.Serialization;
using futura.Util;

namespace futura.pod_dump;

public class Registration : IConfigDefault
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public DateTime? LastProcessed { get; set; }

    public string? TargetLocation { get; set; }
    public string? RelativeLocation { get; set; }
    public string? FilenameConvention { get; set; }

    [JsonIgnore]
    public int PendingEpisodes { get; set; } = 0;

    [JsonIgnore]
    public string? LastProcessedText { get; set; }

    /// <summary>
    /// Used to denote the filename of the config file on the filesystem.
    /// </summary>
    /// <value></value>
    [JsonIgnore]
    public string? ConfigFilename { get; set; } = NewConfigFilename;

    /// <summary>
    /// SHA256 hash of Id + Title to ensure a unique identity.
    /// </summary>
    /// <returns></returns>
    [JsonIgnore]
    public string UniqueId => Hasher.ToSha256($"{ConfigFilename}-{Title}");

    /// <summary>
    /// Centrally manage config file naming scheme.
    /// </summary>
    /// <returns></returns>
    public static string NewConfigFilename => $"{System.Guid.NewGuid()}.json";

    public Registration()
    {

    }

    public Registration(GlobalConfig globalConfig)
    {
        var reg = new Registration
        {
            TargetLocation = globalConfig.TargetLocation,
            RelativeLocation = globalConfig.RelativeLocation,
            FilenameConvention = globalConfig.FilenameConvention
        };
    }


}