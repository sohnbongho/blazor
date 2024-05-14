using AdminApp.Data;
using ArticleApp.Models.Articles;
using ArticleApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdminApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            
            // 게시판 관련 의존성 주입 관련 코드만 따로 모아서 관리
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            AddDependencyInjectionContainerForArticles(builder, connectionString); // Injection

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
        // 게시판 관련 의존성(종속성) 주입 관련 코드만 따로 모아서 관리
        static void AddDependencyInjectionContainerForArticles(WebApplicationBuilder builder, string connectionString)
        {
            // 유저가 만든 DbContext 주입
            // ArticleAppDbContext.cs Inject : New DbContext Add
            builder.Services.AddEntityFrameworkSqlServer().AddDbContext<ArticleAppDbContext>(
                options => options.UseSqlServer(connectionString));

            // IArticleAppDbContext.cs Inject : DI Container에 서비스 등록 (리포지토리)등록
            builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
        }
    }
}