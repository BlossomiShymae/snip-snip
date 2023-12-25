
using System.Reflection;
using BlossomiShymae.SnipSnip.Console;
using McMaster.Extensions.CommandLineUtils;

var app = new CommandLineApplication();
app.HelpOption();

var assembly = Assembly.GetExecutingAssembly();
app.ExtendedHelpText = $@"
{assembly.GetName().Name} {assembly.GetName().Version}
The alternative CommunityDragon directory downloader!
";

await CommandLineApplication.ExecuteAsync<CommandLineInterface>(args);