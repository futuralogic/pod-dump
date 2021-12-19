namespace futura.pod_dump;
public static class ExtractRunner
{

    public static async Task Execute(ExtractOptions options)
    {

        var regman = new RegistrationManager();

        // Build PODCAST filter:
        IEnumerable<Registration> query = regman.Registrations;

        query = query.Where(r => r.PendingEpisodes > 0);

        // Add an order by podcast title.
        query = query.OrderBy(r => r.Title);

        if (!query.Any())
        {
            Out.Text("No podcasts pending extraction.");
            return;
        }

        Out.Line($"Extracting {query.Sum(p => p.PendingEpisodes)} episodes from {query.Count()} podcasts...");
        Out.Line("");

        var pm = new PodcastManager();

        // Retrieve ALL episodes we have registrations for.
        var podcastsWithRegistration = await pm.GetEpisodeData(query.Select(r => r.Uuid));

        // Iterate:
        foreach (var reg in query)
        {

            Out.Line($"{reg.Title} - {reg.PendingEpisodes} episodes");

            // Get pending episodes
            var episodes = pm.GetPendingEpisodes(reg.Uuid, reg.LastProcessed, podcastsWithRegistration);

            foreach (var ep in episodes)
            {

                Out.Line($"   Episode: {ep.Episode}");

                // Determine extraction location
                Out.Line("DEBUG: Generate extraction location");
                var targetTokenized = reg.ExtractionTargetTokenized;

                DateTime epPubDate = DateTime.Parse(ep.EpisodePubDate);

                var data = new TokenMeta
                {
                    PodcastTitle = reg.Title,
                    EpisodeTitle = ep.Episode,
                    EpisodeNumber = Convert.ToString(ep.EpisodeNumber),
                    FileExtension = "mp3", // TODO: Need to fix
                    PublishedYear = epPubDate.Year,
                    PublishedMonth = epPubDate.Month,
                    PublishedDay = epPubDate.Day
                };

                var targetFinal = TokenHandler.ParseTokenizedString(targetTokenized, data);

                System.Diagnostics.Debug.WriteLine($"Target: {targetFinal}");

                if (!options.WhatIf)
                {
                    // Extract
                    Out.Line("DEBUG: Extract (i.e. copy to target) - ");
                }
                else
                {
                    Out.Line("--whatif specified. Skipping actual extraction.");
                }

            }

            if (!options.WhatIf)
            {
                Out.Line("DEBUG: Update registration LastProcessed");
            }
            Out.Line("");
        }

        Out.Line("Extraction complete.");
        return;
    }
}