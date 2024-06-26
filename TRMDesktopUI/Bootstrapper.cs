﻿using Caliburn.Micro;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI;

public class Bootstrapper : BootstrapperBase
{
    private readonly SimpleContainer _container = new();

    public Bootstrapper()
    {
        Initialize();

        ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
    }

    private static IMapper ConfigureAutomapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductModel, ProductDisplayModel>();
            cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
        });

        var output = config.CreateMapper();

        return output;
    }

    private static IConfiguration AddConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

#if DEBUG
        builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

        return builder.Build();
    }

    protected override void Configure()
    {
        _container.Instance(ConfigureAutomapper());

        _container.Instance(_container)
            .PerRequest<IProductEndpoint, ProductEndpoint>()
            .PerRequest<ISaleEndpoint, SaleEndpoint>()
            .PerRequest<IUserEndpoint, UserEndpoint>();

        _container
            .Singleton<IWindowManager, WindowManager>()
            .Singleton<IEventAggregator, EventAggregator>()
            .Singleton<IAPIHelper, APIHelper>()
            .Singleton<ILoggedInUserModel, LoggedInUserModel>();

        _container.RegisterInstance(typeof(IConfiguration), "IConfiguration", AddConfiguration());

        GetType().Assembly.GetTypes()
            .Where(type => type.IsClass)
            .Where(type => type.Name.EndsWith("ViewModel"))
            .ToList()
            .ForEach(viewModelType => _container.RegisterPerRequest(
                viewModelType, viewModelType.ToString(), viewModelType));
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        DisplayRootViewFor<ShellViewModel>();
    }

    protected override object GetInstance(Type service, string key)
    {
        return _container.GetInstance(service, key);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _container.GetAllInstances(service);
    }

    protected override void BuildUp(object instance)
    {
        _container.BuildUp(instance);
    }
}