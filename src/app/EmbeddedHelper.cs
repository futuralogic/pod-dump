using System.Reflection;

public static class EmbeddedHelper
{

	const string BASE_PATH = "resources";
	/// <summary>
	/// Setting this because reflection breaks when trying to typeof(EmbeddedHelper).Namespace - it returns empty string on .net 6
	/// </summary>
	const string DEFAULT_NAMESPACE = "futura.pod_dump";

	/// <summary>
	/// Retrieves the embedded resource.
	/// </summary>
	/// <param name="name">name of resource using periods as folder separator</param>
	/// <returns></returns>
	public static string GetString(string name)
	{
		var fullPath = $"{DEFAULT_NAMESPACE}.{BASE_PATH}.{name}";
		var asm = Assembly.GetExecutingAssembly();

		// foreach (var res in asm.GetManifestResourceNames())
		// {
		// 	Console.WriteLine($"Resource: {res}");
		// }

		if (!asm.GetManifestResourceNames().Contains(fullPath))
		{
			throw new Exception($"The embedded resource called '{fullPath}' was not found.");
		}
		var rs = typeof(EmbeddedHelper).Assembly.GetManifestResourceStream(fullPath);
		if (rs == null) { throw new Exception("Embedded resource was null."); }
		using (var sr = new StreamReader(rs!))
		{
			var data = sr.ReadToEnd();
			sr.Close();
			return data;
		}
	}

}