namespace futura.pod_dump;
public static class ExtractRunner
{

    public static Task Execute(ExtractOptions options)
    {
        Console.WriteLine("Processing...");
        return Task.CompletedTask;
    }
}