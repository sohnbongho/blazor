using Hawaso.Areas.Identity;
using Hawaso.Data;
using Hawaso.Models.Candidates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 새로운 DBContext 추가
//builder.Services.AddDbContext<CandidateAppDbContext>(options => options.UseSqlServer(connectionString));
// DBContext Factory 추가
builder.Services.AddDbContextFactory<CandidateAppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// 개발환경에서 Update-Database, Seed 데이터 추가
if (app.Environment.IsDevelopment())
{
    await CheckCandidateDbMigrated(app.Services);
    CandidateSeedData(app);
    CandidateDbInitializer.Initialize(app);
}

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

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

#region CandidateSeedData: Candidates 테이블에 기본 데이터 입력
// Candidates 테이블에 기본 데이터 입력
static void CandidateSeedData(WebApplication app)
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var candidateDbContext = services.GetRequiredService<CandidateAppDbContext>();

        if (!candidateDbContext.Candidates.Any())
        {
            candidateDbContext.Candidates.Add(new Candidate
            {
                FirstName = "길동",
                LastName = "홍",
                IsEntollment = false,
            });
            candidateDbContext.Candidates.Add(new Candidate
            {
                FirstName = "두산",
                LastName = "백",
                IsEntollment = false,
            });

            candidateDbContext.SaveChanges();
        }
    }
}
#endregion


#region CheckCandidateDbMigrated : 데이터베이스 마이그레이션
// 데이터베이스 마이그레이션
async Task CheckCandidateDbMigrated(IServiceProvider services)
{
    using var scope = services.CreateScope();
    using var candidateContext = scope.ServiceProvider.GetService<CandidateAppDbContext>();
    if (candidateContext is not null)
    {
        await candidateContext.Database.MigrateAsync();
    }
} 
#endregion
