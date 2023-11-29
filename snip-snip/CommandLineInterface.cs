using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using snip_snip.Models;
using snip_snip.Validators;

namespace snip_snip
{
    public class CommandLineInterface
    {
        public readonly HttpClient Http = new();
        public SemaphoreSlim? Slim;

        [Argument(0, Description = "Starting with https://", Name = "URL")]
        [CommunityDragonUrlValidator]
        [Required]
        public string Url { get; } = default!;

        [Option(ShortName = "o", Description = "Output folder to export files to.")]
        public string Output { get; } = "Out";

        [Option(ShortName = "c", Description = "Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3")]
        public int Count { get; } = 20;

        [Option(ShortName = "r", Description = "Retry attempts before giving up on a file.")]
        public int Retry { get; } = 2;

        [Option(ShortName = "", Description = "Fail fast if HTTP GET file bytes request is not successful. Overrides -r|--retry.")]
        public bool FailFast { get; }

        [Option(ShortName = "f", Description = "Force to overwrite existing files instead of skipping.")]
        public bool Force { get; }

        [Option(ShortName = "p", Description = "Pull file listing from the files exported text file so only one request is needed for the directories of files. Requires a bigger initial to load listing.")]
        public bool Pull { get; }

        [Option(ShortName = "", Description = "Filter the list. Non-matching paths are skipped.")]
        public string Filter { get; } = "";

        public async Task OnExecuteAsync()
        {
            DateTime startTime = DateTime.Now;
            var directoryUrl = Url;

            // Limit the download queue.
            Slim = new(Count, Count);
    
            string baseUrl = directoryUrl.Replace(".org", ".org/json");
            string pointerUrl = $"{baseUrl}";
            string outPath = Output;

            // Prepare our folders. <3
            try
            {
                if (Force)
                    Directory.Delete(outPath, true);
            }
            catch (DirectoryNotFoundException) { }
            Directory.CreateDirectory(outPath);

            bool isPull = Pull;
            if (isPull)
            {
                Print($"Scissors ready! --> {pointerUrl}");
                string version = Endpoints.GetVersion(directoryUrl);
                string exportedUrl = Endpoints.GetFilesExported(version);
                byte[] fileBytes = await Http.GetByteArrayAsync(exportedUrl);
                string[] lines = Encoding.Default
                    .GetString(fileBytes)
                    .Split('\n');
                string assetUrl = Endpoints.GetAsset(directoryUrl, version);
                Console.WriteLine(assetUrl);
                List<string> files = lines
                    .Where(x => x.Contains(assetUrl))
                    .ToList();
                // Filter early for better performance
                if (!string.IsNullOrEmpty(Filter))
                {
                     files = files
                        .Where(x => x.Contains(Filter))
                        .ToList();
                }
                Print($"Safe and sound. <-- Pulled {exportedUrl}");

                List<Task> downloadTasks = new();
                foreach (string file in files)
                {
                    pointerUrl = Endpoints.GetJsonPath(file, version);
                    downloadTasks.Add(DownloadFileByNameAsync(baseUrl, pointerUrl, outPath, file.Split('/', StringSplitOptions.RemoveEmptyEntries).Last()));
                }

                await Task.WhenAll(downloadTasks);
            }
            else
            {
                Stack<string> directories = new();
                while (true)
                {
                    Print($"Scissors ready! --> {pointerUrl}");
                    List<CommunityDragonFileInfo>? files = await Http.GetFromJsonAsync<List<CommunityDragonFileInfo>>(pointerUrl);
                    // Empty directory!
                    if (files == null || files.Count == 0)
                        break;
                    await DownloadFilesAsync(baseUrl, pointerUrl, outPath, directories, files);

                    // No more directories!
                    if (directories.Count == 0)
                        break;
                    // Directory is done! Pop stack to point back up or down a directory! >w<
                    pointerUrl = directories.Pop();

                }
            }


            TimeSpan duration = DateTime.Now.Subtract(startTime);
            Print($"Duration: {duration} --- Off we go, scissors!");
        }

        /// Add files to download queue and push any directories into stack.
        async Task DownloadFilesAsync(string baseUrl, string pointerUrl, string outPath, Stack<string> directories, List<CommunityDragonFileInfo> files)
        {
            List<Task> downloadTasks = new();
            foreach (CommunityDragonFileInfo file in files)
            {
                if (file.Type == "directory")
                {
                    // Add to directory stack! :3
                    directories.Push(Path.Join(pointerUrl, file.Name, "/"));
                    continue;
                }

                downloadTasks.Add(DownloadFileAsync(baseUrl, pointerUrl, outPath, file));
            }

            await Task.WhenAll(downloadTasks);
        }

        async Task DownloadFileAsync(string baseUrl, string pointerUrl, string outPath, CommunityDragonFileInfo file)
            => await DownloadFileByNameAsync(baseUrl, pointerUrl, outPath, file.Name);

        /// Download file when queue slot is available.
        async Task DownloadFileByNameAsync(string baseUrl, string pointerUrl, string outPath, string fileName)
        {
            await Slim!.WaitAsync();

            string[] folderPath = pointerUrl
                    .Replace(baseUrl, "")
                    .Split("/", StringSplitOptions.RemoveEmptyEntries)
                    .Prepend(outPath)
                    .ToArray();
            string[] filePath = folderPath
                .Append(fileName)
                .ToArray();

            int attempts = Retry;

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    var outputFilePath = Path.Join(filePath);
                    var pointerPath = Path.Join(pointerUrl, fileName);
                    if (!Force && File.Exists(outputFilePath))
                    {
                        Print($"Skipping! --- {outputFilePath}");
                        continue;
                    }
                    if (!string.IsNullOrEmpty(Filter) && !pointerPath.Contains(Filter))
                    {
                        Print($"Not a match! --- {pointerPath}");
                        continue;
                    }

                    byte[] fileBytes = await Http.GetByteArrayAsync(pointerPath);
                    Directory.CreateDirectory(Path.Join(folderPath));
                    await File.WriteAllBytesAsync(outputFilePath, fileBytes);
                    Print($"Snip! --- {outputFilePath}");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Print($"Bad path! --- {Path.Join(pointerUrl, fileName)}");
                        break;
                    }

                    if (FailFast)
                    {
                        Print($"Failing fast! --- {Path.Join(pointerUrl, fileName)}");
                        Environment.FailFast(ex.ToString());
                    }
                    if (i + 1 == attempts)
                        Print($"Failed to download. --- {Path.Join(pointerUrl, fileName)}");
                    else
                        Print($"Retry attempt #{i + 1}. --- {Path.Join(pointerUrl, fileName)}");
                }
            }
            Slim.Release();
        }

        static void Print(object value) => Console.WriteLine($"{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {value}");
    }
}