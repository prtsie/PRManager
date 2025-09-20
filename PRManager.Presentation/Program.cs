using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<Program>();
var token = builder.Configuration["bot_token"]!;

var bot = new TelegramBotClient(token);
bot.OnMessage += OnMessage;

Console.WriteLine("Running... Press Enter to terminate");
Console.ReadLine();
return;

async Task OnMessage(Message msg, UpdateType type)
{
    var match = MyRegex().Match(msg.Text ?? string.Empty);

    if (!match.Success)
    {
        Console.WriteLine($"Got message: {msg.Text}, no PR found");
    }

    Console.WriteLine($"Got: {msg.Text}. Author: {match.Groups[1].Value}, repo name: {match.Groups[2].Value}, PR number: {match.Groups[3].Value}");
    
    await bot.SendMessage(msg.Chat, "aboba");
}

partial class Program
{
    [GeneratedRegex(@"(?:https?:\/\/)?(?:www\.)?github\.com\/([^\/]+)\/([^\/]+)\/pull\/(\d+)")]
    private static partial Regex MyRegex();
}