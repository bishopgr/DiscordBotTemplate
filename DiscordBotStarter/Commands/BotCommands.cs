using Discord.Commands;

namespace DiscordBotStarter.Commands
{
    public class BotCommands : ModuleBase
    {

        [Command(CommandConstants.HelloWorld, RunMode = RunMode.Async)]
        public async Task HelloWorld()
        {
            await Context.Channel.SendMessageAsync("Hello, world!");
        }
    }
}
