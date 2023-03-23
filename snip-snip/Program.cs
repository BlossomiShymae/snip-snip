using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

var app = new CommandLineApplication();
app.HelpOption();

var assembly = Assembly.GetExecutingAssembly();
app.ExtendedHelpText = $@"
{assembly.GetName().Name} {assembly.GetName().Version}
";

CommandArgument<string> url = app.Argument<string>("URL", "Starting with https://")
	.IsRequired();
url.Validators.Add(new CommunityDragonUrlValidator());
var chunk = app.Option<int>("-c|--count", "Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3 ", CommandOptionType.SingleValue);
chunk.DefaultValue = 20;

app.OnExecuteAsync(async cancellationToken =>
{
	await DownloadFolderAsync(url, cancellationToken);
});

return app.Execute(args);

async Task DownloadFolderAsync(CommandArgument<string> url, CancellationToken cancellationToken)
{
	DateTime startTime = DateTime.Now;
	var directoryUrl = url.Value;
	if (string.IsNullOrEmpty(directoryUrl))
		return;

	var httpClient = new HttpClient();

	string baseUrl = directoryUrl.Replace(".org", ".org/json");
	string pointerUrl = $"{baseUrl}";
	string outPath = "Out";
	Directory.CreateDirectory(outPath);
	Stack<(string Url, CommunityDragonFileInfo File)> directories = new();
	while (true)
	{
		Print($"Scissors ready! --> {pointerUrl}");
		List<CommunityDragonFileInfo>? files = await httpClient.GetFromJsonAsync<List<CommunityDragonFileInfo>>(pointerUrl, cancellationToken: cancellationToken);
		if (files == null || files.Count == 0)
			break;
		await DownloadFilesAsync(httpClient, baseUrl, pointerUrl, outPath, directories, files, cancellationToken);

		if (directories.Count == 0)
			break;
		pointerUrl = directories.Pop().Url;
	}

	TimeSpan duration = DateTime.Now.Subtract(startTime);
	Print($"Duration: {duration} --- Off we go, scissors!");
}

async Task DownloadFilesAsync(HttpClient httpClient, string baseUrl, string pointerUrl, string outPath, Stack<(string Url, CommunityDragonFileInfo File)> directories, List<CommunityDragonFileInfo> files, CancellationToken cancellationToken)
{
	int count = chunk.ParsedValue > 0 ? chunk.ParsedValue : chunk.DefaultValue;
	SemaphoreSlim semaphoreSlim = new(count, count);
	List<Task> downloadTasks = new();
	foreach (CommunityDragonFileInfo file in files)
	{
		if (file.Type == "directory")
		{
			directories.Push((Path.Join(pointerUrl, file.Name, "/"), file));
			continue;
		}

		downloadTasks.Add(DownloadFileAsync(httpClient, baseUrl, pointerUrl, outPath, file, semaphoreSlim, cancellationToken));
	}

	await Task.WhenAll(downloadTasks);
}

async Task DownloadFileAsync(HttpClient httpClient, string baseUrl, string pointerUrl, string outPath, CommunityDragonFileInfo file, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
{
	await semaphoreSlim.WaitAsync(cancellationToken);

	try
	{
		byte[] fileBytes = await httpClient.GetByteArrayAsync(Path.Join(pointerUrl, file.Name), cancellationToken);
		string[] folderPath = pointerUrl
			.Replace(baseUrl, "")
			.Split("/")
			.Prepend(outPath)
			.ToArray();
		string[] filePath = folderPath
			.Append(file.Name)
			.ToArray();

		Directory.CreateDirectory(Path.Join(folderPath));
		await File.WriteAllBytesAsync(Path.Join(filePath), fileBytes, cancellationToken);
		Print($"Snip! --- {Path.Join(filePath)}");
	}
	finally
	{
		semaphoreSlim.Release();
	}

}

static void Print(object value) => Console.WriteLine($"{DateTimeOffset.Now.ToUniversalTime()} {value}");

record CommunityDragonFileInfo
{
	[JsonPropertyName("name")]
	public string Name { get; init; } = default!;
	[JsonPropertyName("type")]
	public string Type { get; init; } = default!;
	[JsonPropertyName("mtime")]
	public string MTime { get; init; } = default!;
	[JsonPropertyName("size")]
	public long? Size { get; init; }
}

class CommunityDragonUrlValidator : IArgumentValidator
{
	public ValidationResult? GetValidationResult(CommandArgument argument, ValidationContext context)
	{
		var value = argument.Value;
		if (string.IsNullOrEmpty(value))
			return ValidationResult.Success;

		if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
			return new ValidationResult($"The URL must be a valid URI string");
		var uri = new Uri(value);
		if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
			return new ValidationResult($"The URL must be using http/https protocol");
		if (!value.Contains("communitydragon.org"))
			return new ValidationResult($"The URL must be a valid CommunityDragon link");
		if (!value.EndsWith('/'))
			return new ValidationResult($"The URL must be a directory");

		return ValidationResult.Success;
	}
}