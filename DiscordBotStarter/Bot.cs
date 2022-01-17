using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotStarter.Settings;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DiscordBotStarter
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IDiscordSettings _discordSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Bot> _logger;
        public Bot(DiscordSocketClient client, CommandService commands, IDiscordSettings discordSettings, IServiceProvider serviceProvider, ILogger<Bot> logger)
        {
            _client = client;
            _commands = commands;
            _discordSettings = discordSettings ?? throw new ArgumentNullException(nameof(discordSettings));
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.WriteLine(message.Source);
            Console.ResetColor();

            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(IServiceProvider services)
        {

            _client.Log += Log;
            _commands.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _discordSettings.Token);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            _client.MessageReceived += HandleCommandAsync;
            await Task.Delay(Timeout.Infinite);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;


            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(_client, msg);

                _logger.LogInformation($"Executing {context.Message}.");

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await _commands.ExecuteAsync(context, pos, _serviceProvider);

                if (!result.IsSuccess)
                {
                    await msg.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
                }

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                //    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }


        }
    }
}
