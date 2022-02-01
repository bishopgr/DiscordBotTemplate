using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotStarter
{
    public class StartBot
    {
        public static async Task Start(IServiceProvider services)
        {
            using var serviceScope = services.CreateScope();

            var provider = serviceScope.ServiceProvider;

            var robotStarter = provider.GetRequiredService<Bot>();

            await robotStarter.ConnectAsync(services);
        }
    }
}
