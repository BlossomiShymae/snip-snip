using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlossomiShymae.SnipSnip.Desktop.ViewModels;
using BlossomiShymae.SnipSnip.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BlossomiShymae.SnipSnip.Desktop;

public partial class App(IServiceProvider serviceProvider) : Application
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}