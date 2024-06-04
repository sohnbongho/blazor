namespace SignalRStudy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Keep Alive�ð� ����
            builder.Services.AddSignalR().AddHubOptions<MessageHub>(options =>
            {
                options.EnableDetailedErrors = true;
                // 30�� �ʰ���, �������� ������ �������
                options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
            });


            var app = builder.Build();
            app.UseDefaultFiles(); // wwwroot������� ����ϰڴ�.
            app.UseStaticFiles();

            app.MapGet("/", () => "Hello World!");
            app.MapHub<MessageHub>("/mychat");

            app.Run();
        }
    }
}
