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
        var podcastsWithRegistration = (await pm.GetEpisodeData(query.Select(r => r.Uuid))).ToList();

        // Iterate:
        var registrationsToProcess = query.ToList();
        foreach (var reg in registrationsToProcess)
        {

            Out.Line($"{reg.Title} - {reg.PendingEpisodes} episodes");

            // Get pending episodes
            var episodes = pm.GetPendingEpisodes(reg.Uuid, reg.LastProcessed, podcastsWithRegistration);

            foreach (var ep in episodes)
            {

                Out.Line($"  Episode: {ep.Episode}");

                // Determine extraction location
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
                    PublishedDay = epPubDate.Day,
                    Author = ep.AUTHOR
                };

                var targetFinal = TokenHandler.ParseTokenizedString(targetTokenized, data);

                // Extract
                var source = new Uri(ep.Url).LocalPath;
                var dest = targetFinal;

                System.Diagnostics.Debug.WriteLine($"  > Source: {source}");
                System.Diagnostics.Debug.WriteLine($"  > Dest: {dest}");


                if (File.Exists(dest))
                {
                    Out.Line($"   > Target already exists. Not overwriting. (Target: {dest})");
                }

                if (!options.WhatIf)
                {
                    if (!File.Exists(dest))
                    {
                        var path = Path.GetDirectoryName(dest);
                        System.Diagnostics.Debug.WriteLine($"   > Creating dir to {path}");
                        Directory.CreateDirectory(path);
                        File.Copy(source, dest);
                        System.Diagnostics.Debug.WriteLine($"   > Copy complete.");

                        // TODO: Tag resulting audio file.
                        Out.Line("DEBUG: Tag target audio file.");
                    }
                }
                else
                {
                    Out.Line("   > --whatif specified. Skipping actual extraction.");
                    Out.Line($"   > Source: {source}");
                    Out.Line($"   > Dest: {dest}");

                }

            }

            if (!options.WhatIf)
            {
                reg.LastProcessed = DateTime.Now;
                await regman.UpdateRegistation(reg);
            }

            Out.Line("");
        }

        Out.Line("Extraction complete.");
        return;
    }
}