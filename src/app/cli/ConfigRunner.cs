namespace futura.pod_dump;
public static class ConfigRunner
{

    public static async Task Execute(ConfigOptions options)
    {
        var cfg = new ConfigManager();

        if (options.ResetConfiguration)
        {

            Out.Line("Resetting global configuration to defaults.");

            await cfg.ResetGlobalConfig();

            return;
        }

        if (options.UninstallConfiguration)
        {
            Out.Line("Removing entire app configuration.");
            ConfigManager.CleanupApp();

            return;
        }

        if (!string.IsNullOrEmpty(options.TargetLocation))
        {
            var config = cfg.AppConfiguration;

            config.TargetLocation = options.TargetLocation;

            await cfg.SaveGlobalConfig(config);

        }

        if (!string.IsNullOrEmpty(options.RelativeLocation))
        {
            var config = cfg.AppConfiguration;

            config.RelativeLocation = options.RelativeLocation;

            await cfg.SaveGlobalConfig(config);

        }

        if (!string.IsNullOrEmpty(options.FilenameConvention))
        {
            var config = cfg.AppConfiguration;

            config.FilenameConvention = options.FilenameConvention;

            await cfg.SaveGlobalConfig(config);

        }

        Out.Line("Current Configuration");
        Out.Line("---------------------");

        Out.Line($"Target Location      : {cfg.AppConfiguration.TargetLocation}");
        Out.Line($"Relative Location    : {cfg.AppConfiguration.RelativeLocation}");
        Out.Line($"File Name Convention : {cfg.AppConfiguration.FilenameConvention}");

        return;
    }

}