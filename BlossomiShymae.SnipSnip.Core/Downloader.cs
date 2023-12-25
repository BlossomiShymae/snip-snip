using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlossomiShymae.SnipSnip.Core
{
    public class Downloader
    {
        protected HttpClient Http = new();
        protected SemaphoreSlim Slim = new(1, 1);

        public required string Url { get; init; }
        public int Count { get; init; } = 20;
        public bool FailFast { get; init ; }
        public string Filter { get; init; } = string.Empty;
        public bool Force { get; init; }
        public int MaxDepth { get; init; } = 0;
        public string Output { get; init; } = "Out";
        public bool Pull { get; init; }
        public int Retry { get; init; } = 2;

        /// <summary>
        /// Default values shared with projects.
        /// </summary>
        public static class Defaults
        {
            public static readonly int Count = 20;
            public static readonly bool FailFast = false;
            public static readonly string Filter = string.Empty;
            public static readonly bool Force = false;
            public static readonly int MaxDepth = 0;
            public static readonly string Output = "Out";
            public static readonly bool Pull = false;
            public static readonly int Retry = 2;
        }

        /// <summary>
        /// Default tooltips / descriptions shared with projects.
        /// </summary>
        public static class Tips
        {
            public const string Count = "Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3";
            public const string FailFast = "Fail fast if HTTP GET file bytes request is not successful. Overrides retry option.";
            public const string Filter = "Filter the list. Non-matching paths are skipped.";
            public const string Force = "Force to overwrite existing files instead of skipping.";
            public const string MaxDepth = "The maximum depth for files and directories. 0 is recursive.";
            public const string Output = "Output folder for exported files.";
            public const string Pull = "Pull file listing from files.exported.txt to use instead of JSON listing. Requires an initial download to load listing.";
            public const string Retry = "Retry attempts before giving up on a file.";

        }

        public required Action<string> PrintHandler { get; init; }
        public required Action FailFastHandler { get; init; }

        public Downloader()
        {
            Slim = new(Count, Count);
        }

        public async Task ExecuteAsync()
        {
            var startTime = DateTime.Now;

            CreateDirectory();
            Print($"Scissors ready! --> {Url}");

            if (Pull)
                await RunWithExportedFiles();
            else
                await RunWithJsonFiles();
            
            var duration = DateTime.Now.Subtract(startTime);
            Print($"Duration: {duration} --- Off we go, scissors!");
        }

        private async Task RunWithJsonFiles()
        {
            var directories = new Stack<string>();
            var pointerUrl = Url.Replace(".org", ".org/json");
            while (true)
            {
                Print($"Go! --> {pointerUrl}");
                var files = await Http.GetFromJsonAsync<List<FileInfo>>(pointerUrl);
                // Empty directory
                if (files == null || files.Count == 0)
                    break;

                await RunDownloadTasksAsync(pointerUrl, directories, files);
                
                // No more directories
                if (directories.Count == 0)
                    break;
                // Directory is done
                pointerUrl = directories.Pop();
            }
        }

        private async Task RunDownloadTasksAsync(string pointerUrl, Stack<string> directories, List<FileInfo> files)
        {
            var basePath = Url.Replace(".org", ".org/json");
            var downloadTasks = new List<Task>();
            foreach (var file in files)
            {
                if (file.Type == "directory")
                {
                    // Check against max depth
                    if (MaxDepth > 0)
                    {
                        // Get the path difference between base path and pointer path
                        var diff = Path
                            .Join(pointerUrl, file.Name)
                            .Replace(basePath, string.Empty);
                        // Get depth count
                        var depth = diff
                            .Split('/', StringSplitOptions.RemoveEmptyEntries)
                            .Length;
                        if (depth >= MaxDepth)
                            continue;
                    }
                    directories.Push(Path.Join(pointerUrl, file.Name, "/"));
                    continue;
                }

                downloadTasks.Add(DownloadFileAsync(basePath, pointerUrl, file.Name));
            }

            await Task.WhenAll(downloadTasks);
        }

        private async Task DownloadFileAsync(string basePath, string pointerUrl, string name)
        {
            await Slim.WaitAsync();

            var folderPath = pointerUrl
                .Replace(basePath, string.Empty)
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Prepend(Output)
                .ToArray();
            var filePath = folderPath
                .Append(name)
                .ToArray();
            
            for (int i = 0; i < Retry; i++)
            {
                try
                {
                    var outputFilePath = Path.Join(filePath);
                    var pointerPath = Path.Join(pointerUrl, name);
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

                    var fileBytes = await Http.GetByteArrayAsync(pointerPath);
                    Directory.CreateDirectory(Path.Join(folderPath));
                    await File.WriteAllBytesAsync(outputFilePath, fileBytes);
                    Print($"Snip! --- {outputFilePath}");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Print(ex.Message);
                    var pointerPath = Path.Join(pointerUrl, name);

                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Print($"Bad path! --- {pointerPath}");
                        break;
                    }

                    if (FailFast)
                    {
                        Print($"Failing fast! --- {pointerPath}");
                        FailFastHandler.Invoke();
                    }

                    if (i + 1 == Retry)
                        Print($"Failed to download. --- {pointerPath}");
                    else
                        Print($"Retry attempt #{i + 1}. --- {pointerPath}");

                }
            }

            Slim.Release();
        }

        private async Task RunWithExportedFiles()
        {
            var basePath = Url.Replace(".org", ".org/json");
            var asset = new AssetPath(Url);
            var fileBytes = await Http.GetByteArrayAsync(asset.FilesExportedPath);
            var lines = Encoding.Default
                .GetString(fileBytes)
                .Split('\n');
            var files = lines
                .Where(x => x.Contains(asset.Path));
            
            // Filter early for better performance
            if (!string.IsNullOrEmpty(Filter))
            {
                files = files
                    .Where(x => x.Contains(Filter))
                    .ToList();
            }

            Print($"Safe and sound. <-- Pulled {asset.FilesExportedPath}");
            var downloadTasks = new List<Task>();
            foreach (var file in files)
            {
                var pointerPath = asset.GetJsonPath(file);
                // Check against max depth
                if (MaxDepth > 0)
                {
                    // Get the path difference between base path and pointer path
                    var diff = pointerPath.Replace(basePath, string.Empty);
                    // Get depth count
                    var depth = diff
                        .Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .Length;
                    if (depth >= MaxDepth)
                        continue;
                }

                var fileName = file
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Last();
                downloadTasks.Add(DownloadFileAsync(basePath, pointerPath, fileName));
            }

            await Task.WhenAll(downloadTasks);
        }

        private void CreateDirectory()
        {
            try
            {
                if (Force)
                    Directory.Delete(Output, true);
            }
            catch (DirectoryNotFoundException) {}
            Directory.CreateDirectory(Output);
        }

        private void Print(string value)
        {
            PrintHandler.Invoke($"{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {value}");
        }
    }
}