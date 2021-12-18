using CommandLine;
namespace futura.pod_dump;

[Verb("remove", HelpText = "Remove a podcast registration using its ID (use list -x for ID).")]
public class RemoveOptions
{
    [Option("id", Required = true, HelpText = "Unique id of the podcast registration to remove.")]
    public string Id { get; set; } = string.Empty;
}