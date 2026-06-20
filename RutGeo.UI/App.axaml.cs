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
        Console.WriteLine("App: Initializing...");
        AvaloniaXamlLoader.Load(this);
        Console.WriteLine("App: Initialize completed.");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("App: OnFrameworkInitializationCompleted started...");
        var serviceCollection = CreateServiceCollection();
        Services = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("App: Setting up MainWindow...");
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            Console.WriteLine("App: MainWindow created.");
        }

        base.OnFrameworkInitializationCompleted();
        Console.WriteLine("App: OnFrameworkInitializationCompleted finished.");
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
        serviceCollection.AddTransient<IExplanationLog, ExplanationLog>();
        serviceCollection.AddTransient<IRutEquationGenerator, RutEquationGenerator>();
        serviceCollection.AddTransient<IEquationTransformer, EquationTransformer>();
    }
    
    private void RegisterViewModels(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<MainWindowViewModel>();
    }
}