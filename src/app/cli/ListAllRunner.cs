namespace futura.pod_dump;
public static class ListAllRunner
{

	public static Task Execute(ListAllOptions options)
	{
		Console.WriteLine("Processing...");
		return Task.CompletedTask;
	}

}