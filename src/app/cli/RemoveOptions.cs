using CommandLine;

[Verb("remove", HelpText = "Remove a podcast registration using its title.")]
public class RemoveOptions
{
	[Option('s', "search", Required = true, HelpText = "Full or partial podcast title to find.")]
	public string? Title { get; set; }

	[Option('x', "exact", HelpText = "Force exact matching of the podcast title.")]
	public bool ExactMatch { get; set; }

}