using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlossomiShymae.SnipSnip.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlossomiShymae.SnipSnip.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // URL
        [ObservableProperty]
        private string _url = string.Empty;

        // COUNT
        [ObservableProperty]
        private int _count = Downloader.Defaults.Count;
        [ObservableProperty]
        private string _countTip = Downloader.Tips.Count;

        // FAIL FAST
        [ObservableProperty]
        private bool _failFast = Downloader.Defaults.FailFast;
        [ObservableProperty]
        private string _failFastTip = Downloader.Tips.FailFast;

        // FILTER
        [ObservableProperty]
        private string _filter = Downloader.Defaults.Filter;
        [ObservableProperty]
        private string _filterTip = Downloader.Tips.Filter;

        // FORCE
        [ObservableProperty]
        private bool _force = Downloader.Defaults.Force;
        [ObservableProperty]
        private string _forceTip = Downloader.Tips.Force;

        // MAX DEPTH
        [ObservableProperty]
        private int _maxDepth = Downloader.Defaults.MaxDepth;
        [ObservableProperty]
        private string _maxDepthTip = Downloader.Tips.MaxDepth;

        // OUTPUT
        [ObservableProperty]
        private string _output = Downloader.Defaults.Output;
        [ObservableProperty]
        private string _outputTip = Downloader.Tips.Output;

        // PULL
        [ObservableProperty]
        private bool _pull = Downloader.Defaults.Pull;
        [ObservableProperty]
        private string _pullTip = Downloader.Tips.Pull;

        // RETRY
        [ObservableProperty]
        private int _retry = Downloader.Defaults.Retry;
        [ObservableProperty]
        private string _retryTip = Downloader.Tips.Retry;

        [ObservableProperty]
        private string _log = string.Empty;

        [RelayCommand]
        private async Task StartAsync()
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
                FailFastHandler = () => { throw new InvalidOperationException(); },
            };
            var validator = new DownloaderValidator();

            var results = validator.Validate(downloader);

            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                    Print(error.ErrorMessage);
                return;
            }

            try
            {
                await downloader.ExecuteAsync();
            } catch (InvalidOperationException)
            {
                Print("Failed fast.");
            }
        }

        public void Print(string value)
        {
           Log += $"{value}\n";
        }
    }
}