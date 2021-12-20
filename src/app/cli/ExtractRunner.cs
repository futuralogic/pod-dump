namespace futura.pod_dump;
public static class ExtractRunner
{

    public static async Task Execute(ExtractOptions options)
    {

        var regman = new RegistrationManager();

        // Build PODCAST filter:
        IEnumerable<Registration> query = regman.Registrations;

        query = query.Where(r => r.PendingEpisodes > 0);

        if (!query.Any())
        {
            Out.Text("No podcasts pending extraction.");
            return;
        }

        Out.Line($"Extracting {query.Sum(p => p.PendingEpisodes)} episodes from {query.Count()} podcasts...");
        Out.Line("");

        var pm = new PodcastManager();

        // Retrieve ALL podcast episodes for podcasts we have registrations for.
        // Pass in our list of Podcast UUId's and retrieve the list of episodes from Sqlite:
        var podcastsWithRegistration = (await pm.GetEpisodeData(query.Select(r => r.Uuid))).ToList();

        // Add an order by podcast title.
        query = query.OrderBy(r => r.Title);

        // Iterate:
        foreach (var reg in query.ToList())
        {
            //Out.Line($"DEBUG: reg count: {registrationsToProcess.Count()}");

            Out.Line($"{reg.Title} - {reg.PendingEpisodes} episodes");

            // Get pending episodes
            var episodes = pm.GetPendingEpisodes(reg.Uuid, reg.LastProcessed, podcastsWithRegistration);

            foreach (var ep in episodes)
            {

                Out.Line($"  Episode: {ep.Episode}");

                // Determine extraction location
                var targetTokenized = reg.ExtractionTargetTokenized;

                DateTime epPubDate = DateTime.Parse(ep.EpisodePubDate);

                // Extract
                var source = new Uri(ep.Url).LocalPath;

                var data = new TokenMeta
                {
                    PodcastTitle = reg.Title,
                    EpisodeTitle = ep.Episode,
                    EpisodeNumber = Convert.ToString(ep.EpisodeNumber),
                    FileExtension = Path.GetExtension(source),
                    PublishedYear = epPubDate.Year,
                    PublishedMonth = epPubDate.Month,
                    PublishedDay = epPubDate.Day,
                    Author = ep.Author
                };

                var dest = TokenHandler.ParseTokenizedString(targetTokenized, data);

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

                        // ***************************************************************
                        // DIRECTORY CREATE + FILE COPY
                        System.Diagnostics.Debug.WriteLine($"   > Creating dir to {path}");

                        Directory.CreateDirectory(path!);
                        File.Copy(source, dest);

                        System.Diagnostics.Debug.WriteLine($"   > Copy complete.");
                        // ***************************************************************

                        var tagFile = TagLib.File.Create(dest);

                        tagFile.Tag.AlbumArtists = new string[] { data.Author };
                        tagFile.Tag.Performers = new string[] { data.Author };

                        tagFile.Tag.Album = data.PodcastTitle;
                        tagFile.Tag.Title = data.EpisodeTitle;

                        tagFile.Tag.DateTagged = DateTime.Now;

                        tagFile.Tag.Year = Convert.ToUInt16(epPubDate.Year);

                        if (!string.IsNullOrEmpty(data.EpisodeNumber) && data.EpisodeNumber.Trim() != "0")
                        {
                            tagFile.Tag.Track = Convert.ToUInt16(data.EpisodeNumber);
                        }

                        tagFile.Save();

                        System.Diagnostics.Debug.WriteLine($"   > Tagging complete.");

                    }

                }
                else
                {
                    Out.Line("   > --whatif specified. Skipping actual extraction.");
                    Out.Line($"   > Source: {source}");
                    Out.Line($"   > Dest: {dest}");
                    Out.Line($"   > Tags:");
                    Out.Line($"         - Album Artist: {data.Author}");
                    Out.Line($"         - Artist: {data.Author}");
                    Out.Line($"         - Album: {data.PodcastTitle}");
                    Out.Line($"         - Title: {data.EpisodeTitle}");
                    Out.Line($"         - Year: {epPubDate.Year}");
                    if (!string.IsNullOrEmpty(data.EpisodeNumber) && data.EpisodeNumber.Trim() != "0")
                    {
                        Out.Line($"         - Track #: {Convert.ToUInt16(data.EpisodeNumber)}");
                    }
                }
            }

            if (!options.WhatIf)
            {
                reg.LastProcessed = DateTime.Now;
                regman.UpdateRegistation(reg);
                System.Diagnostics.Debug.WriteLine($"   > Updated last processed date.");
            }

            Out.Line("");
        }

        if (!options.WhatIf)
        {
            // Update configs in filesystem to record the extraction.
            regman.SaveRegistrationConfigs();
            System.Diagnostics.Debug.WriteLine($"Registration configs have been updated.");
        }

        Out.Line("Extraction complete.");
        return;
    }
}