using CommandLine;
namespace futura.pod_dump;

[Verb("find", HelpText = "Finds podcasts in Apple's library.")]
public class FindOptions
{
    [Option('s', "search", Required = true, HelpText = "Full or partial podcast title to find.")]
    public string Title { get; set; } = string.Empty;

    [Option('x', "exact", HelpText = "Force exact matching of the podcast title.")]
    public bool ExactMatch { get; set; }
}