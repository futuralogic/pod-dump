using CommandLine;

namespace futura.pod_dump;

[Verb("list", HelpText = "Lists registered podcasts.")]
public class ListOptions
{

    [Option('p', "pending", HelpText = "Only show podcasts with pending episodes.")]
    public bool Pending { get; set; }

    [Option('x', "extended", HelpText = "Display additional registration details (such as ID - used to remove a registration).")]
    public bool ShowExtended { get; set; }

    [Option("raw", Hidden = true, HelpText = "Show raw details.")]
    public bool ShowRaw { get; set; }
}