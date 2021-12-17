public static class AddRunner
{

	public static Task Execute(AddOptions options)
	{
		Console.WriteLine("Processing add...");
		return Task.CompletedTask;
	}

}