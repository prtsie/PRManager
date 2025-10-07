using PRManager.Approving.Web;
using PRManager.Notifying.Web;

namespace PRManager.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureApproving(builder.Configuration);
        builder.Services.ConfigureNotifying(builder.Configuration);
        builder.Services.RegisterAutoMapper();
        builder.Services.ConfigureLogger();
        builder.Services.AddControllers();
        var githubSecret = builder.Configuration["GithubSecret"]!;

        var app = builder.Build();

        app.UseApproving(githubSecret);
        app.MapControllers();

        app.Run();
    }
}