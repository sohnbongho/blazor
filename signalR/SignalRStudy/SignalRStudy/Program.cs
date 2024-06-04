namespace SignalRStudy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            var app = builder.Build();
            app.MapGet("/", () => "Hello World!");
            app.MapHub<MessageHub>("/mychat");

            app.Run();
        }
    }
}
