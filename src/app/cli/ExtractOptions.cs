using CommandLine;
namespace futura.pod_dump;

[Verb("extract", HelpText = "Extracts the podcasts, sets audio tags, and saves them to the specified target location.")]
public class ExtractOptions
{

    [Option("whatif", HelpText = "Display a summary of what would happen if extraction occurred.")]
    public bool WhatIf { get; set; }
}