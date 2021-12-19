namespace futura.pod_dump;
public static class AddRunner
{

    public async static Task Execute(AddOptions options)
    {

        var regman = new RegistrationManager();

        var searchTerm = options.Title;
        var isSearchExact = options.ExactMatch;

        // Check whether this podcast is already registered.
        var reglist = regman.FindRegistrations(searchTerm!, isSearchExact);
        var regCount = reglist.Count();

        if (regCount > 0)
        {
            var multiple = regCount > 1;

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

            Out.Line("");
        }

        // Find podcasts - get metadata from SQL Lite
        var pm = new PodcastManager();
        var results = await pm.FindPodcast(searchTerm, isSearchExact);

        var resultsToAdd = results.Where(p => !reglist.Any(r => r.Uuid.Equals(p.PodcastUUId)));

        if (!resultsToAdd.Any())
        {
            Out.Line($"No additional podcasts found matching: {searchTerm}");
            return;
        }

        if (resultsToAdd.Count() == 1)
        {
            Out.Line("Found a new podcast to register:");
        }
        else
        {
            Out.Line($"Found {resultsToAdd.Count()} new podcasts to register:");
        }

        var cfgman = new ConfigManager();

        foreach (var found in resultsToAdd)
        {
            var newReg = new Registration(cfgman.AppConfiguration);

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

            newReg.Uuid = Guid.Parse(found.PodcastUUId);
            newReg.Title = found.Podcast;

            Out.Line($"Registering new: {found.Podcast}");

            // Write podcast meta file to registrations folder
            await regman.AddRegistration(newReg);

        }

        return;
    }

}