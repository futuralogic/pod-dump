public static class ConfigRunner
{

	public static Task Execute(ConfigOptions options)
	{

		if (options.Current)
		{

			var cfg = new ConfigManager();

			Out.Line("Current Configuration");
			Out.Line("---------------------");

			Out.Line($"Target Location      : {cfg.AppConfiguration.TargetLocation}");
			Out.Line($"Relative Location    : {cfg.AppConfiguration.RelativeLocation}");
			Out.Line($"File Name Convention : {cfg.AppConfiguration.FilenameConvention}");

		}

		if (options.UninstallConfiguration)
		{
			var cfg = new ConfigManager();
			Out.Line("Removing entire app configuration.");
			cfg.CleanupApp();
		}

		return Task.CompletedTask;
	}

}