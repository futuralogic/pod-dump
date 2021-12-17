namespace futura.pod_dump;
public static class FindRunner
{

    public static async Task Execute(FindOptions options)
    {
        var searchTerm = options.Title;
        var isSearchExact = options.ExactMatch;

        var pm = new PodcastManager();
        var results = await pm.FindPodcast(searchTerm, isSearchExact);

        if (!results.Any())
        {
            Out.Line($"No podcasts found matching search term: {searchTerm}");
            return;
        }

        foreach (var found in results)
        {
            Out.Line($"Found: {found.Podcast}");
        }
    }
}
