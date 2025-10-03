using PRManager.Approving.Web;

namespace PRManager.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureApproving(builder.Configuration);
        builder.Services.RegisterAutoMapper();
        var githubSecret = builder.Configuration["GithubSecret"]!;

        var app = builder.Build();

        app.UseApproving(githubSecret);

        app.Run();
    }
}