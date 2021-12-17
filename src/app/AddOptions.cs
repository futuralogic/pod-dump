using CommandLine;

[Verb("add")]
public class AddOptions
{
	[Option(Required = true)]
	public string? Title { get; set; }

	public bool? ExactMatch { get; set; }

	public string? CustomTargetLocation { get; set; }

	public string? CustomRelativeLocation { get; set; }

	public string? CustomFilenameConvention { get; set; }

}