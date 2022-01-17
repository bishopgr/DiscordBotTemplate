using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotStarter.Settings
{
    //Add whatever settings you need here for your discord bot.
    public class DiscordSettings : IDiscordSettings
    {
        public string Token { get; set; }
    }
}
