namespace futura.pod_dump;
public static class RemoveRunner
{
    public static Task Execute(RemoveOptions options)
    {
        var regman = new RegistrationManager();

        if (regman.HasRegistration(options.Id))
        {
            try
            {
                var reg = regman.GetRegistration(options.Id);
                regman.RemoveRegistration(options.Id);
                Out.Line($"Registration for '{reg?.Title}' removed.");
            }
            catch (Exception problem)
            {
                Out.Line("An error occurred while trying to remove the registration.");
                Out.Line($"Error: {problem.Message}");
                Out.Line("Stack:");
                Out.Line(problem.ToString());
            }

        }
        else
        {
            Out.Line($"Registration '{options.Id}' not found.");
        }

        return Task.CompletedTask;
    }

}