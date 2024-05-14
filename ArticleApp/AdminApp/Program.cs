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
            
            // �Խ��� ���� ������ ���� ���� �ڵ常 ���� ��Ƽ� ����
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
        // �Խ��� ���� ������(���Ӽ�) ���� ���� �ڵ常 ���� ��Ƽ� ����
        static void AddDependencyInjectionContainerForArticles(WebApplicationBuilder builder, string connectionString)
        {
            // ������ ���� DbContext ����
            // ArticleAppDbContext.cs Inject : New DbContext Add
            builder.Services.AddEntityFrameworkSqlServer().AddDbContext<ArticleAppDbContext>(
                options => options.UseSqlServer(connectionString));

            // IArticleAppDbContext.cs Inject : DI Container�� ���� ��� (�������丮)���
            builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
        }
    }
}