
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Services;
using ViewModels;

namespace MainEntry;

public class Bootstrapper
{
    public Bootstrapper()
    {
        var services = ConfigureServices();
        Ioc.Default.ConfigureServices(services);
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Services
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        // AddTransient: 요청할 때마다 새로운 인스턴스 생성
        services.AddTransient<ShellViewModel>();
        services.AddTransient<MainViewModel>();

        services.AddTransient<AlbumArtInfoViewModel>();

        return services.BuildServiceProvider();
    }
}
