using CommandLine;

var result = Parser.Default.ParseArguments<AddOptions>(args)
	.WithParsed<AddOptions>(async o => await AddRunner.Execute(o));


