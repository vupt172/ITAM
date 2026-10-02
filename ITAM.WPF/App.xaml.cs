using AutoUpdaterDotNET;
using ITAM.AppCore.Interfaces;
using ITAM.AppCore.Mappings;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using ITAM.Infrastructure.Services;
using ITAM.ViewModels;
using ITAM.WPF;
using ITAM.WPF.Services;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Catalogs;
using ITAM.WPF.ViewModels.Reports;
using ITAM.WPF.Views;
using ITAM.WPF.Views.Dialogs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;
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
            services.AddTransient<ITaiSanDinhDanhService, TaiSanDinhDanhService>();
            services.AddTransient<IVatTuService, VatTuService>();
            services.AddScoped<IDieuChuyenService, DieuChuyenService>();
            services.AddScoped<ILichSuDieuChuyenService, LichSuDieuChuyenService>();
            services.AddSingleton<IFileStorageService, FileStorageService>();
            services.AddTransient<IBaoCaoTaiSanService, BaoCaoTaiSanService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<ISoTaiSanCoDinhService, SoTaiSanCoDinhExporter>();
            services.AddTransient<IBaoCaoTongHopTaiSanService, BaoCaoTongHopTaiSanService>();

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
            services.AddTransient<TaiSanDinhDanhListViewModel>();
            services.AddTransient<TaiSanDinhDanhEditViewModel>();
            services.AddTransient<VatTuListViewModel>();
            services.AddTransient<VatTuEditViewModel>();
            services.AddTransient<DieuChuyenViewModel>();
            services.AddTransient<DieuChuyenSearchViewModel>();
            services.AddTransient<LichSuDieuChuyenViewModel>();
            services.AddTransient<HeThongBaoCaoViewModel>();
            services.AddTransient<BaoCaoTaiSanViewModel>();
            services.AddTransient<PhongBanViTriTreeViewModel>();
            services.AddTransient<PhongBanViTriTreeWindow>();
            services.AddTransient<SoTaiSanCoDinhViewModel>();
            services.AddTransient<TongHopTaiSanViewModel>();

            // DI Windows
            services.AddTransient<LoginWindow>();              // ⬅ đổi Transient (mở lại được khi logout)
            services.AddSingleton<MainWindow>();
            services.AddTransient<AddEditUserWindow>();
            services.AddTransient<AddEditRoleWindow>();
            services.AddTransient<ChangePhongBanWindow>();
            services.AddTransient<LoNhapSearchWindow>();
            services.AddTransient<TaiSanDinhDanhEditWindow>();
            services.AddTransient<VatTuEditWindow>();
            services.AddTransient<DieuChuyenView>();
            services.AddTransient<DieuChuyenSearchWindow>();

            Services = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
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
            AutoUpdater.Start(@"\\192.168.1.4\Publish\ITAM\Update.xml");

            // Mở LoginWindow trước, MainWindow chỉ hiện sau khi đăng nhập thành công
            var loginWindow = Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Đã xảy ra lỗi không mong muốn:\n\n{e.Exception.Message}\n\nỨng dụng sẽ tiếp tục chạy, nhưng nên lưu lại công việc và khởi động lại nếu gặp lỗi tiếp theo.",
                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

            // Danh dau da xu ly -> KHONG cho app tat, chi bao loi va tiep tuc chay
            e.Handled = true;
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Loi xay ra ngoai UI thread (vd trong Task.Run) khong the "Handled" duoc,
            // nhung it nhat ghi lai duoc truoc khi process chet that su.
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Lỗi nghiêm trọng: {ex.Message}", "Lỗi nghiêm trọng",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
