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

            try
            {
                var result = Parser.Default.ParseArguments<AddOptions, RemoveOptions, ListOptions, ExtractOptions, ConfigOptions, FindOptions>(args)
                    .WithParsed<AddOptions>(async o => await AddRunner.Execute(o))
                    .WithParsed<RemoveOptions>(async o => await RemoveRunner.Execute(o))
                    .WithParsed<ListOptions>(async o => await ListRunner.Execute(o))
                    .WithParsed<ExtractOptions>(async o => await ExtractRunner.Execute(o))
                    .WithParsed<ConfigOptions>(async o => await ConfigRunner.Execute(o))
                    .WithParsed<FindOptions>(async o => await FindRunner.Execute(o));

            }
            catch (Exception too_bad_for_you)
            {
                Out.Line($"Error: {too_bad_for_you.Message}");
                Out.Line("Stack:");
                Out.Line(too_bad_for_you.ToString());
                return -1;
            }

            return 0;
        }
    }
}
