using CommandLine;
namespace futura.pod_dump
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // Verify our configuration is available or intact.
            var cm = new ConfigManager();

            await cm.CheckAndInitGlobalConfig();

            var result = Parser.Default.ParseArguments<AddOptions, RemoveOptions, ListAllOptions, ListOptions, ExtractOptions, ConfigOptions>(args)
                .WithParsed<AddOptions>(async o => await AddRunner.Execute(o))
                .WithParsed<RemoveOptions>(async o => await RemoveRunner.Execute(o))
                .WithParsed<ListAllOptions>(async o => await ListAllRunner.Execute(o))
                .WithParsed<ListOptions>(async o => await ListRunner.Execute(o))
                .WithParsed<ExtractOptions>(async o => await ExtractRunner.Execute(o))
                .WithParsed<ConfigOptions>(async o => await ConfigRunner.Execute(o));

            return 0;
        }
    }
}
