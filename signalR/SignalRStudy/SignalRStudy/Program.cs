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

            // Keep Alive시간 설정
            builder.Services.AddSignalR().AddHubOptions<MessageHub>(options =>
            {
                options.EnableDetailedErrors = true;
                // 30초 초과시, 서버에서 연결을 끊어버림
                options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
            });

            // Akka.net 관련 추가
            builder.Services.AddSingleton<ActorSystem>(provider =>
            {
                var actorSystem = ActorSystem.Create("MyActorSystem");                
                return actorSystem;
            });            
            builder.Services.AddHostedService<AkkaService>();


            var app = builder.Build();
            app.UseDefaultFiles(); // wwwroot기반으로 사용하겠다.
            app.UseStaticFiles();

            app.MapGet("/", () => "Hello World!");
            app.MapHub<MessageHub>("/mychat");

            app.Run();
        }
    }
}
