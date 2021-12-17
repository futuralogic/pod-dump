public static class ConfigRunner
{

	public static Task Execute(ConfigOptions options)
	{
		Console.WriteLine("Processing...");
		return Task.CompletedTask;
	}

}