using ITAM.AppCore.Interfaces;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using ITAM.Infrastructure.Services;
using ITAM.WPF.Services;
using ITAM.WPF.UserControls;
using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Catalogs;
using ITAM.WPF.Views.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace ITAM.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public App()
        {
            IConfiguration configuration =
            new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            // Khai báo DI container và đăng ký các dịch vụ, ViewModel, và Window
            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );
            // DI Services
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<IPhongBanService, PhongBanService>();

            // DI ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MenuBarViewModel>();
            services.AddSingleton<ToolbarViewModel>();

            services.AddTransient<DashboardViewModel>();
            services.AddTransient<HeThongDMViewModel>();
            services.AddTransient<DMTaiSanViewModel>();
            services.AddTransient<PhongBanViewModel>();
            services.AddTransient<NhaCungCapViewModel>();
            services.AddTransient<LoaiTaiSanViewModel>();
            services.AddTransient<ViTriTaiSanViewModel>();

            // DI Windows
            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
     
            // Lấy MainWindow từ DI container và hiển thị nó
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

    }
}

