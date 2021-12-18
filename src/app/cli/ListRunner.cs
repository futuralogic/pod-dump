namespace futura.pod_dump;
public static class ListRunner
{
    // Number of characters in the title
    const int TITLE_WIDTH = 45;
    const int AVAILABLE_WIDTH = 14;
    const int LASTPROCESSED_WIDTH = 19;
    const int EXTRACT_WIDTH = 40;

    const string HEADER = "Podcast                                      Available     Last Processed     Target";

    const int ID_WIDTH = 7;

    const string HEADER_EXTENDED = "ID     Podcast                                      Available     Last Processed     Target";

    public static Task Execute(ListOptions options)
    {
        return RenderList(options);
    }

    static Task RenderList(ListOptions options)
    {
        Out.Line(options.ShowExtended ? HEADER_EXTENDED : HEADER);

        var cfg = new ConfigManager();

        var hasAnyShown = false;

        foreach (var reg in cfg.Registrations)
        {
            // Find the number of episodes that need to be extracted since "LastProcessed"
            var numberOfEpisodes = 0;
            if (options.Pending && numberOfEpisodes == 0)
            {
                // Don't show a podcast if it doesn't have pending episodes
                continue;
            }

            // We have shown an entry.
            hasAnyShown = true;

            if (options.ShowExtended)
            {
                var idToFit = reg.UniqueId.Substring(0, 6).ToLower();
                Out.Text(idToFit.PadRight(ID_WIDTH));
            }

            var titleToFit = reg.Title.Length > (TITLE_WIDTH - 4) ? $"{reg.Title.Substring(0, TITLE_WIDTH - 4)}..." : reg.Title;
            // TITLE
            Out.Text(titleToFit.PadRight(TITLE_WIDTH));
            // AVAILABLE COUNT
            Out.Text("2".PadRight(AVAILABLE_WIDTH));
            // LAST PROCESSED DATE
            // Calculate "relative" time for this date.
            Out.Text("7 days ago".PadRight(LASTPROCESSED_WIDTH));
            // EXTRACTION TARGET
            Out.Text("{expanded_target_string}".PadRight(EXTRACT_WIDTH));
            Out.Line("");
        }

        if (options.Pending && !hasAnyShown)
        {
            Out.Text("No podcasts pending extraction.");
        }

        return Task.CompletedTask;
    }

}