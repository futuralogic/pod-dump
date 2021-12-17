public static class ListRunner
{

	public static Task Execute(ListOptions options)
	{
		Console.WriteLine("Processing...");
		return Task.CompletedTask;
	}

}