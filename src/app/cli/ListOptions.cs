using CommandLine;

namespace futura.pod_dump;

[Verb("list", HelpText = "Lists registered podcasts with pending episodes that haven't been processed (extracted).")]
public class ListOptions
{
    [Option('a', "all", HelpText = "List all registered podcasts.")]
    public bool ShowAll { get; set; }

    [Option('x', "extended", HelpText = "Display additional registration details (such as ID - used to remove a registration).")]
    public bool ShowExtended { get; set; }

    [Option("raw", Hidden = true, HelpText = "Show raw details.")]
    public bool ShowRaw { get; set; }
}