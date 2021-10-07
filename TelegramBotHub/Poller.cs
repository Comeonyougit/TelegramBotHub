using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBotHub
{
    public class Poller
    {
        public Poller(string accessToken)
        {
            BotClient = new TelegramBotClient(accessToken);
        }

        public async Task Init()
        {
            var updates = await BotClient.GetUpdatesAsync();
        }
    }
}
