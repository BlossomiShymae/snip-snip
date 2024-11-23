using BlossomiShymae.Smolder;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;

CoconaApp.Run(async (
    [Argument(Description = "Starting with https://", Name = "URL")]
    string url, 
    [Option(Description = "Filter the list. Non-matching paths are skipped.")]
    string filter = "", 
    [Option('c', Description = "Concurrent download queue size controlled by semaphore")]
    int count = 20, 
    [Option(Description = "The maximum depth for files and directories. 0 is recursive.")]
    int maxDepth = 0, 
    [Option('o', Description = "Output folder for downloaded files.")]
    string output = "out", 
    [Option('f', Description = "Force to delete the output folder if it exists.")]
    bool overwrite = true, 
    [Option('s', Description = "Skip existing files. When disabled, overwrite existing files.")]
    bool skip = false,
    [Option('r', Description = "Retry attempts before giving up on a file.")]
    int retry = 3) =>
{
    var logger  = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} {Level:w4}: {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    var client  = new Client()
    {
        Logger = LoggerFactory.Create(builder => builder.AddSerilog(logger)).CreateLogger<Client>(),
        Filter = filter,
        ConcurrentDownloadCount = count,
        MaxDepth = maxDepth,
        OutputPath = output,
        OverwriteFolder = overwrite,
        SkipFile = skip,
        Retries = retry
    };

    await client.DownloadDirectoryAsync(url);
});