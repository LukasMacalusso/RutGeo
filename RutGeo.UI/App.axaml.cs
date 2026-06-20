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
using RutGeo.Core.Services.Validation;
using RutGeo.Core.Services.Common;
using RutGeo.Core.Services.Conics;
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
        serviceCollection.AddSingleton<IExplanationLogger, ExplanationLogger>();
        serviceCollection.AddTransient<IConicElementsFactory, ConicElementsFactory>();
        serviceCollection.AddTransient<IEquationTransformer, EquationTransformer>();
    }

    private void RegisterViewModels(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<MainWindowViewModel>();
    }
}
