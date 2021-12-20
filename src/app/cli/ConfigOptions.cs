using CommandLine;
namespace futura.pod_dump;
[Verb("config", HelpText = "Set application defaults.")]
public class ConfigOptions
{
    const string DEFAULT_FILENAME_CONVENTION = @"{podcast} - {episode}{ext}";

    [Option('c', "current", Group = "config", HelpText = "Display current configuration.")]
    public bool Current { get; set; }

    [Option('t', "targetlocation", Group = "config", HelpText = "Base path to folder location you want podcasts to be extracted to (ex: ~/Music). Default: Current Directory")]
    public string? TargetLocation { get; set; }

    [Option('r', "relativelocation", Group = "config", HelpText = "Relative folder location under the {TargetLocation} to extract to. (ex: \"{podcast}\" would extract an mp3 called \"Episode1.mp3\" to ~/Music/A Podcast Title/Episode1.mp3). Use format specifiers to create dynamic paths. Default: Not specified.")]
    public string? RelativeLocation { get; set; }

    [Option('f', "filenameconvention", Group = "config", HelpText = "Default file naming convention of extracted podcasts. Default: " + DEFAULT_FILENAME_CONVENTION)]
    public string? FilenameConvention { get; set; }

    [Option("reset", Group = "config", HelpText = "Reset configuration to defaults.")]
    public bool ResetConfiguration { get; set; }

    [Option('u', "uninstall", Group = "config", HelpText = "Removes app configuration.")]
    public bool UninstallConfiguration { get; set; }


}