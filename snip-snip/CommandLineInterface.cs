using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using snip_snip.Models;
using snip_snip.Validators;

namespace snip_snip
{
    public class CommandLineInterface
    {
        [Argument(0, Description = "Starting with https://", Name = "URL")]
        [CommunityDragonUrlValidator]
        [Required]
        public string Url { get; } = default!;

        [Option(Description = "Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3", Template = "-c|--count")]
        public int Count { get; } = 20;

        [Option(ShortName = "r", Description = "Retry attempts before giving up on a file.")]
        public int Retry { get; } = 2;

        [Option(ShortName = "f", Description = "Fail fast if HTTP GET file bytes request is not successful. Overrides -r|--retry.")]
        public bool FailFast { get; }

        [Option(ShortName = "p", Description = "Pull file listing from the files exported text file so only one request is needed for the directories of files. Requires a bigger initial to load listing.")]
        public bool Pull { get; }

        public async Task OnExecuteAsync()
        {
            DateTime startTime = DateTime.Now;
            var directoryUrl = Url;
            if (string.IsNullOrEmpty(directoryUrl))
                return;

            var httpClient = new HttpClient();

            string baseUrl = directoryUrl.Replace(".org", ".org/json");
            string pointerUrl = $"{baseUrl}";
            string outPath = "Out";

            // Prepare our folders. <3
            try
            {
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
                byte[] fileBytes = await httpClient.GetByteArrayAsync(exportedUrl);
                string[] lines = Encoding.Default
                    .GetString(fileBytes)
                    .Split('\n');
                string assetUrl = Endpoints.GetAsset(directoryUrl, version);
                List<string> files = lines.Where(x => x.Contains(assetUrl)).ToList();
                Print($"Safe and sound. <-- Pulled {exportedUrl}");

                // Limit the download queue.
                SemaphoreSlim semaphoreSlim = new(Count, Count);
                List<Task> downloadTasks = new();
                foreach (string file in files)
                {
                    pointerUrl = Endpoints.GetJsonPath(file, version);
                    downloadTasks.Add(DownloadFileByNameAsync(httpClient, baseUrl, pointerUrl, outPath, file.Split('/', StringSplitOptions.RemoveEmptyEntries).Last(), semaphoreSlim));
                }

                await Task.WhenAll(downloadTasks);
            }
            else
            {
                Stack<string> directories = new();
                while (true)
                {
                    Print($"Scissors ready! --> {pointerUrl}");
                    List<CommunityDragonFileInfo>? files = await httpClient.GetFromJsonAsync<List<CommunityDragonFileInfo>>(pointerUrl);
                    // Empty directory!
                    if (files == null || files.Count == 0)
                        break;
                    await DownloadFilesAsync(httpClient, baseUrl, pointerUrl, outPath, directories, files);

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
        async Task DownloadFilesAsync(HttpClient httpClient, string baseUrl, string pointerUrl, string outPath, Stack<string> directories, List<CommunityDragonFileInfo> files)
        {
            // Limit the download queue.
            SemaphoreSlim semaphoreSlim = new(Count, Count);
            List<Task> downloadTasks = new();
            foreach (CommunityDragonFileInfo file in files)
            {
                if (file.Type == "directory")
                {
                    // Add to directory stack! :3
                    directories.Push(Path.Join(pointerUrl, file.Name, "/"));
                    continue;
                }

                downloadTasks.Add(DownloadFileAsync(httpClient, baseUrl, pointerUrl, outPath, file, semaphoreSlim));
            }

            await Task.WhenAll(downloadTasks);
        }

        async Task DownloadFileAsync(HttpClient httpClient, string baseUrl, string pointerUrl, string outPath, CommunityDragonFileInfo file, SemaphoreSlim semaphoreSlim)
            => await DownloadFileByNameAsync(httpClient, baseUrl, pointerUrl, outPath, file.Name, semaphoreSlim);

        /// Download file when queue slot is available.
        async Task DownloadFileByNameAsync(HttpClient httpClient, string baseUrl, string pointerUrl, string outPath, string fileName, SemaphoreSlim semaphoreSlim)
        {
            await semaphoreSlim.WaitAsync();

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
                    byte[] fileBytes = await httpClient.GetByteArrayAsync(Path.Join(pointerUrl, fileName));
                    Directory.CreateDirectory(Path.Join(folderPath));
                    await File.WriteAllBytesAsync(Path.Join(filePath), fileBytes);
                    Print($"Snip! --- {Path.Join(filePath)}");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(ex);
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
            semaphoreSlim.Release();
        }

        static void Print(object value) => Console.WriteLine($"{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {value}");
    }
}