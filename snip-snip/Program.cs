using McMaster.Extensions.CommandLineUtils;
using snip_snip;
using System.Reflection;

var app = new CommandLineApplication();
app.HelpOption();

var assembly = Assembly.GetExecutingAssembly();
app.ExtendedHelpText = $@"
{assembly.GetName().Name} {assembly.GetName().Version}
An alternate Community Dragon Directory Downloader.
";

await CommandLineApplication.ExecuteAsync<CommandLineInterface>(args);