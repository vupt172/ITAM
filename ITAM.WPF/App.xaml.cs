using AutoUpdaterDotNET;
using ITAM.AppCore.Interfaces;
using ITAM.AppCore.Mappings;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using ITAM.Infrastructure.Services;
using ITAM.WPF;
using ITAM.WPF.Services;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Catalogs;
using ITAM.WPF.Views;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
namespace ITAM.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ViTriTaiSanMappingConfig).Assembly);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient);   // ⬅ thêm tham số này

            // DI Models
            services.AddSingleton<ICurrentUserContext, CurrentUserContext>();

            // DI Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddSingleton<IErrorDialogService, ErrorDialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient(typeof(ICatalogService<>), typeof(CatalogService<>));
            services.AddTransient<IViTriTaiSanService, ViTriTaiSanService>();
            services.AddTransient<IHangHoaService, HangHoaService>();
            services.AddTransient<ILoNhapService, LoNhapService>();
            services.AddSingleton<IToolbarService, ToolbarService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IFeatureService, FeatureService>();

            // DI ViewModels
            services.AddTransient<LoginViewModel>();          // ⬅ thêm
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
            services.AddTransient<HangHoaViewModel>();
            services.AddTransient<LoNhapViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<AddEditUserViewModel>();
            services.AddTransient<RoleManagementViewModel>();
            services.AddTransient<AddEditRoleViewModel>();
            services.AddTransient<ThamSoNguoiDungViewModel>();
            services.AddTransient<ChangePhongBanViewModel>();
            services.AddTransient<LoNhapSearchViewModel>();
            // DI Windows
            services.AddTransient<LoginWindow>();              // ⬅ đổi Transient (mở lại được khi logout)
            services.AddSingleton<MainWindow>();
            services.AddTransient<AddEditUserWindow>();
            services.AddTransient<AddEditRoleWindow>();
            services.AddTransient<ChangePhongBanWindow>();
            services.AddTransient<LoNhapSearchWindow>();
            Services = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Seed dữ liệu Feature/Role/Admin trước khi hiển thị bất kỳ Window nào
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    await DbSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khởi tạo dữ liệu hệ thống: {ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
            }
            AutoUpdater.Start(@"\\192.168.1.4\Public\ITAM\Publish\Update.xml");

            // Mở LoginWindow trước, MainWindow chỉ hiện sau khi đăng nhập thành công
            var loginWindow = Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
