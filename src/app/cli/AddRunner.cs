namespace futura.pod_dump;
public static class AddRunner
{

    public async static Task Execute(AddOptions options)
    {

        var cfg = new ConfigManager();

        var searchTerm = options.Title;
        var isSearchExact = options.ExactMatch;

        // Check whether this podcast is already registered.
        var reglist = cfg.FindRegistrations(searchTerm!, isSearchExact);
        var multiple = reglist.Count >= 1;
        if (reglist.Count > 0)
        {
            if (multiple)
            {
                Out.Line("Existing podcast registrations match your search criteria.");
            }
            else
            {
                Out.Line("An existing podcast registration matches your search criteria.");
            }
            int count = 1;
            foreach (var reg in reglist)
            {
                Out.Line($"{count++} {reg.Title}");
            }
            return;
        }

        // Find podcasts - get metadata from SQL Lite
        var pm = new PodcastManager();
        var results = await pm.FindPodcast(searchTerm, isSearchExact);

        if (!results.Any())
        {
            Out.Line($"No podcasts found matching search term: {searchTerm}");
            return;
        }

        foreach (var found in results)
        {

            var newReg = new Registration(cfg.AppConfiguration);

            if (!string.IsNullOrEmpty(options.CustomTargetLocation))
            {
                newReg.TargetLocation = options.CustomTargetLocation;
            }

            if (!string.IsNullOrEmpty(options.CustomRelativeLocation))
            {
                newReg.RelativeLocation = options.CustomRelativeLocation;
            }

            if (!string.IsNullOrEmpty(options.CustomFilenameConvention))
            {
                newReg.FilenameConvention = options.CustomFilenameConvention;
            }

            newReg.Id = found.Id;
            newReg.Title = found.Podcast;

            Out.Line($"Registering: {found.Podcast}");

            // Write podcast meta file to registrations folder
            await cfg.AddRegistration(newReg);

        }

        return;
    }

}