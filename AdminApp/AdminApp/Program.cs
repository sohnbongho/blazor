using AdminApp.Areas.Identity;
using AdminApp.Data;
using AdminApp.Infrastructures;
using AdminApp.Models;
using DotNetNote.Models;
using Hawaso.Models.CommonValues;
using Hawaso.Models.Notes;
using Hawaso.Models;
using Hawaso.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using VisualAcademy;

namespace AdminApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var services = builder.Services;
        var Configuration = builder.Configuration;

        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure()));

        // ASP.NET Core Identity 설정
        services.AddIdentity<IdentityUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 4;
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            options.SlidingExpiration = true;
        });

        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        //builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        // Add services to the container.
        services.AddControllersWithViews(); // View에서 가져옴

        services.AddRazorPages();
        services.AddServerSideBlazor()
            .AddCircuitOptions(options => { options.DetailedErrors = true; });

        // CORS 설정
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        services.AddHttpClient();

        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddSingleton<WeatherForecastService>();

        services.Configure<DotNetNoteSettings>(Configuration.GetSection("DotNetNoteSettings"));

        services.AddDbContext<DotNetNoteContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<NoteDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // 의존성 주입 컨테이너 설정
        //DependencyInjectionContainer(builder);

        //services.AddTransient<IFileStorageManager, ReplyAppFileStorageManager>();
        //services.AddDependencyInjectionContainerForPurgeApp(Configuration.GetConnectionString("DefaultConnection"));

        //// 데이터베이스 초기화 및 마이그레이션
        //try
        //{
        //    await DatabaseHelper.AddOrUpdateRegistrationDate(connectionString);
        //}
        //catch (Exception)
        //{

        //}

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.MapRazorPages();

        //using (var scope = app.Services.CreateScope())
        //{
        //    var scopedServices = scope.ServiceProvider;
        //    await UserAndRoleInitializer.CreateBuiltInUsersAndRoles(scopedServices);
        //}

        app.Run();
    }
    // 의존성 주입 메서드 정의
    private static void DependencyInjectionContainer(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var Configuration = builder.Configuration;

        services.AddSingleton<IConfiguration>(Configuration);

        services.AddTransient<INoteRepository, NoteRepository>();
        services.AddTransient<INoteCommentRepository, NoteCommentRepository>();

        services.AddDbContext<HawasoDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
        services.AddDbContext<CommonValueDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddTransient<ICommonValueRepository, CommonValueRepository>();

        // ManufacturerApp 관련 의존성 주입
        //services.AddDependencyInjectionContainerForManufacturer(Configuration.GetConnectionString("DefaultConnection"));

        //// Blazored.Toast
        //services.AddBlazoredToast();

        //// MemoApp 관련 의존성 주입
        //services.AddDependencyInjectionContainerForMemoApp(Configuration.GetConnectionString("DefaultConnection"));

        //// Upload Feature
        //services.AddDiForLibries(Configuration.GetConnectionString("DefaultConnection"));
        //services.AddDiForBriefingLogs(Configuration.GetConnectionString("DefaultConnection"));

        //// ArchiveApp 관련 의존성 주입
        //services.AddDependencyInjectionContainerForArchiveApp(Configuration.GetConnectionString("DefaultConnection"));

        //#region VisualAcademy.Models.Departments.dll 
        //// DepartmentApp 관련 의존성 주입
        //services.AddDependencyInjectionContainerForDepartmentApp(Configuration.GetConnectionString("DefaultConnection"));
        //#endregion

        //builder.Services.AddDependencyInjectionContainerForBannedTypeApp(connectionString);
    }
}