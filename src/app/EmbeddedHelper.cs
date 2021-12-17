using System.Reflection;

namespace futura.pod_dump;

public static class EmbeddedHelper
{

	const string BASE_PATH = "resources";

	/// <summary>
	/// Retrieves the embedded resource.
	/// </summary>
	/// <param name="name">name of resource using periods as folder separator</param>
	/// <returns></returns>
	public static string GetString(string name)
	{
		var fullPath = $"{BASE_PATH}.{name}".ToLower();
		var asm = Assembly.GetExecutingAssembly();

		/*foreach (var res in asm.GetManifestResourceNames())
		{
			Console.WriteLine($"Resource: {res}");
		}*/

		if (!asm.GetManifestResourceNames().Any(s => s.Contains(fullPath)))
		{
			throw new Exception($"The embedded resource called '{fullPath}' was not found.");
		}
		var rs = typeof(EmbeddedHelper).Assembly.GetManifestResourceStream(typeof(EmbeddedHelper), fullPath);
		if (rs == null) { throw new Exception($"Embedded resource '{fullPath}' was null."); }
		else
		{
			using (var sr = new StreamReader(rs!))
			{
				var data = sr.ReadToEnd();
				sr.Close();
				return data;
			}
		}
	}

}