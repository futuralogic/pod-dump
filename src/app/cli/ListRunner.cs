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

    const int CONFIG_WIDTH = 42;
    const int ZUUID_WIDTH = 40;

    const string HEADER_EXTENDED_RAW = "ID     Podcast                                      Config                                    ZUUID";

    public static Task Execute(ListOptions options)
    {
        return RenderList(options);
    }

    static Task RenderList(ListOptions options)
    {
        if (!options.ShowRaw)
        {
            Out.Line(options.ShowExtended ? HEADER_EXTENDED : HEADER);
        }
        else
        {
            Out.Line(HEADER_EXTENDED_RAW);
        }

        var regman = new RegistrationManager();

        // Build PODCAST filter:
        IEnumerable<Registration> query = regman.Registrations;
        if (!options.ShowAll)
        {
            query = query.Where(r => r.PendingEpisodes >= 1);
        }
        // Add an order by podcast title.
        query = query.OrderBy(r => r.Title);

        if (!options.ShowAll && !query.Any())
        {
            Out.Text("No podcasts pending extraction.");
        }

        // Iterate:
        foreach (var reg in query)
        {
            if (options.ShowExtended || options.ShowRaw)
            {
                var idToFit = reg.UniqueId.Substring(0, 6).ToLower();
                Out.Text(idToFit.PadRight(ID_WIDTH));
            }

            var titleToFit = reg.Title.Length > (TITLE_WIDTH - 4) ? $"{reg.Title.Substring(0, TITLE_WIDTH - 4)}..." : reg.Title;
            // TITLE
            Out.Text(titleToFit.PadRight(TITLE_WIDTH));

            if (!options.ShowRaw)
            {
                // AVAILABLE COUNT
                Out.Text(reg.PendingEpisodes.ToString().PadRight(AVAILABLE_WIDTH));
                // LAST PROCESSED DATE
                var last = !string.IsNullOrEmpty(reg.LastProcessedText) ? reg.LastProcessedText.PadRight(LASTPROCESSED_WIDTH) : new string(' ', LASTPROCESSED_WIDTH);
                Out.Text(last);
                // EXTRACTION TARGET
                Out.Text("{expanded_target_string}".PadRight(EXTRACT_WIDTH));
            }
            else
            {
                // CONFIG FILE NAME
                Out.Text(Path.GetFileName(reg.ConfigFilename).PadRight(CONFIG_WIDTH));
                // ZUUID

                var uuid = reg.Uuid.ToString();
                Out.Text(uuid.PadRight(ZUUID_WIDTH));

            }

            Out.Line("");
        }

        return Task.CompletedTask;
    }
}