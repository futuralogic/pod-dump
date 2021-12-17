public class GlobalConfig : IConfigDefault
{
	public const string DEFAULT_FILENAME_CONVENTION = "{podcast}{ - }{title}.{ext}";

	public string? TargetLocation { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
	public string? RelativeLocation { get; set; } = string.Empty;
	public string? FilenameConvention { get; set; } = DEFAULT_FILENAME_CONVENTION;

}