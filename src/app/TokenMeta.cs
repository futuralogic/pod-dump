namespace futura.pod_dump;

public class TokenMeta
{
    [Token("podcast")]
    public string? PodcastTitle { get; set; }

    [Token("episode")]
    public string? EpisodeTitle { get; set; }

    [Token("ext")]
    public string? FileExtension { get; set; }

    [Token("episode_number")]
    public string? EpisodeNumber { get; set; }

    [Token("year")]
    public int PublishedYear { get; set; }

    [Token("month")]
    public int PublishedMonth { get; set; }

    [Token("day")]
    public int PublishedDay { get; set; }

    public string? Author { get; set; }

}

[AttributeUsage(validOn: AttributeTargets.Property)]
public class TokenAttribute : System.Attribute
{
    public string TokenKey { get; protected set; }

    public TokenAttribute(string Key)
    {
        TokenKey = Key;
    }
}