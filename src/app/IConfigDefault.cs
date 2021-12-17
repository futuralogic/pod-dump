namespace futura.pod_dump;

interface IConfigDefault
{
	string? TargetLocation { get; set; }
	string? RelativeLocation { get; set; }
	string? FilenameConvention { get; set; }
}