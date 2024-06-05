using Akka.Actor;
using SignalRStudy.Services;

namespace SignalRStudy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // Keep Alive�ð� ����
            builder.Services.AddSignalR().AddHubOptions<MessageHub>(options =>
            {
                options.EnableDetailedErrors = true;
                // 30�� �ʰ���, �������� ������ �������
                options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
            });

            // Akka.net ���� �߰�
            builder.Services.AddSingleton<ActorSystem>(provider =>
            {
                var actorSystem = ActorSystem.Create("MyActorSystem");                
                return actorSystem;
            });            
            builder.Services.AddHostedService<AkkaService>();


            var app = builder.Build();
            app.UseDefaultFiles(); // wwwroot������� ����ϰڴ�.
            app.UseStaticFiles();

            app.MapGet("/", () => "Hello World!");
            app.MapHub<MessageHub>("/mychat");

            app.Run();
        }
    }
}
