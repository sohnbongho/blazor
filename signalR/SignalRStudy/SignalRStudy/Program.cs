namespace SignalRStudy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Keep Alive시간 설정
            builder.Services.AddSignalR().AddHubOptions<MessageHub>(options =>
            {
                options.EnableDetailedErrors = true;
                // 30초 초과시, 서버에서 연결을 끊어버림
                options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
            });


            var app = builder.Build();
            app.UseDefaultFiles(); // wwwroot기반으로 사용하겠다.
            app.UseStaticFiles();

            app.MapGet("/", () => "Hello World!");
            app.MapHub<MessageHub>("/mychat");

            app.Run();
        }
    }
}
