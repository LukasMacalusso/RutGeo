using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RutGeo.Core.Interfaces;
using RutGeo.Core.Services;
using RutGeo.UI.ViewModels;
using RutGeo.UI.Views;

namespace RutGeo.UI;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = CreateServiceCollection();
        Services = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private ServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        
        RegisterCoreServices(serviceCollection);
        RegisterViewModels(serviceCollection);

        return serviceCollection;
    }

    private void RegisterCoreServices(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRutValidator, RutValidator>();
    }
    
    private void RegisterViewModels(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<MainWindowViewModel>();
    }
}