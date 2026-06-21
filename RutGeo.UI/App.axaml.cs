using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RutGeo.Core.Interfaces.Validation;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Generators;
using RutGeo.Core.Interfaces.Functions;
using RutGeo.Core.Services.Validation;
using RutGeo.Core.Services.Common;
using RutGeo.Core.Services.Conics;
using RutGeo.Core.Services.Generators;
using RutGeo.Core.Services.Functions;
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
        serviceCollection.AddSingleton<IExplanationLogger, ExplanationLogger>();
        serviceCollection.AddTransient<IConicElementsFactory, ConicElementsFactory>();
        serviceCollection.AddTransient<IRutEquationGenerator, RutEquationGenerator>();
        serviceCollection.AddTransient<IEquationTransformer, EquationTransformer>();
        serviceCollection.AddTransient<IFunctionAnalyzer, FunctionAnalyzer>();
        serviceCollection.AddTransient<IConicOrchestrator, ConicOrchestrator>();
        serviceCollection.AddTransient<ILimitOrchestrator, LimitOrchestrator>();
    }

    private void RegisterViewModels(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<MainWindowViewModel>();
    }
}