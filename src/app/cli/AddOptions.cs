using CommandLine;
namespace futura.pod_dump;
[Verb("add", HelpText = "Register a podcast using its title.")]
public class AddOptions
{
	[Option('s', "search", Required = true, HelpText = "Full or partial podcast title to find.")]
	public string? Title { get; set; }

	[Option('x', "exact", HelpText = "Force exact matching of the podcast title.")]
	public bool ExactMatch { get; set; }

	[Option('t', "to", HelpText = "Override the default `TargetLocation` configuration setting for this registration.")]
	public string? CustomTargetLocation { get; set; }

	[Option('r', "rel", HelpText = "Override the default `RelativeLocation` configuration setting for this registration.")]
	public string? CustomRelativeLocation { get; set; }

	[Option('f', "file", HelpText = "Override the default `FilenameConvention` configuration setting for this registration.")]
	public string? CustomFilenameConvention { get; set; }

}