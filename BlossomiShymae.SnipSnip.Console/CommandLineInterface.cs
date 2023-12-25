using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlossomiShymae.SnipSnip.Core;
using McMaster.Extensions.CommandLineUtils;

namespace BlossomiShymae.SnipSnip.Console
{
    public class CommandLineInterface
    {
        [Argument(0, Description = "Starting with https://", Name = "URL")]
        [Required]
        public string Url { get; } = default!;

        [Option(ShortName = "c", Description = Downloader.Tips.Count)]
        public int Count { get; } = Downloader.Defaults.Count;

        [Option(ShortName = "", Description = Downloader.Tips.FailFast)]
        public bool FailFast { get; } = Downloader.Defaults.FailFast;

        [Option(ShortName = "", Description = Downloader.Tips.Filter)]
        public string Filter { get; } = Downloader.Defaults.Filter;

        [Option(ShortName = "f", Description = Downloader.Tips.Force)]
        public bool Force { get; } = Downloader.Defaults.Force;

        [Option(ShortName = "", Description = Downloader.Tips.MaxDepth)]
        public int MaxDepth { get; } = Downloader.Defaults.MaxDepth;

        [Option(ShortName = "o", Description = Downloader.Tips.Output)]
        public string Output { get; } = Downloader.Defaults.Output;

        [Option(ShortName = "p", Description = Downloader.Tips.Pull)]
        public bool Pull { get; } = Downloader.Defaults.Pull;

        [Option(ShortName = "r", Description = Downloader.Tips.Retry)]
        public int Retry { get; } = Downloader.Defaults.Retry;

        public async Task OnExecuteAsync()
        {
            var downloader = new Downloader()
            {
                Url = Url,
                Count = Count,
                FailFast = FailFast,
                Filter = Filter,
                Force = Force,
                MaxDepth = MaxDepth,
                Output = Output,
                Pull = Pull,
                Retry = Retry,
                PrintHandler = Print,
                FailFastHandler = Fail
            };
            var validator = new DownloaderValidator();

            var results = validator.Validate(downloader);
            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                    System.Console.WriteLine(error.ErrorMessage);
                return;
            }

            await downloader.ExecuteAsync();
        }

        public static void Print(string value) => System.Console.WriteLine(value);

        public static void Fail() => Environment.FailFast(string.Empty);
    }
}