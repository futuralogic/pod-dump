using System.Text.Json;
using System.Text.Json.Serialization;

namespace futura.pod_dump;

public class Registration : IConfigDefault
{
    public string? Id { get; set; }
    public string? Title { get; set; }

    public DateTime? LastProcessed { get; set; }

    public string? TargetLocation { get; set; }
    public string? RelativeLocation { get; set; }
    public string? FilenameConvention { get; set; }

    /// <summary>
    /// Used to denote the filename of the config file on the filesystem.
    /// </summary>
    /// <value></value>
    [JsonIgnore]
    public string? ConfigFilename { get; set; } = NewConfigFilename;

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

    /// <summary>
    /// Centrally manage config file naming scheme.
    /// </summary>
    /// <returns></returns>
    public static string NewConfigFilename => $"{System.Guid.NewGuid()}.json";

}