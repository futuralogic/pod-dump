namespace futura.pod_dump;
public static class RemoveRunner
{
	public static Task Execute(RemoveOptions options)
	{
		Console.WriteLine("Processing...");
		return Task.CompletedTask;
	}

}