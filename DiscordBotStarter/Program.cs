//// See https://aka.ms/new-console-template for more information

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotStarter;
using DiscordBotStarter.Commands;
using DiscordBotStarter.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;



using IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices((_, services) =>
{
    services.AddLogging();
    services.AddSingleton<BotCommands>();
    services.AddSingleton(c => new CommandService(new CommandServiceConfig
    {
        LogLevel = LogSeverity.Info,
        CaseSensitiveCommands = false
    }));
    services.AddSingleton<DiscordSocketClient>();
    services.AddSingleton<Bot>();

    var dir = Directory.GetCurrentDirectory();

    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
    //Create a file called discord-conf.json (or whatever), and add your token there. DO NOT HARDCODE THE TOKEN!
    configurationBuilder.SetBasePath(dir).AddJsonFile("discord-conf.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

    IConfigurationRoot configurationRoot = configurationBuilder.Build();

    var section = configurationRoot.GetSection(nameof(DiscordSettings));

    services.Configure<DiscordSettings>(section);

    services.AddSingleton<IDiscordSettings>(ds => ds.GetRequiredService<IOptions<DiscordSettings>>().Value);

}).Build();

await StartBot.Start(host.Services);

await host.RunAsync();

