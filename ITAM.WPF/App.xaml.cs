using ITAM.AppCore.Interfaces;
using ITAM.WPF.Services;
using ITAM.WPF.UserControls;
using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Catalogs;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ITAM.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Khai báo DI container và đăng ký các dịch vụ, ViewModel, và Window
            var services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MenuBarViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddTransient<HeThongDMViewModel>();

            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();
            // Lấy MainWindow từ DI container và hiển thị nó
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

    }
}

