using System.Threading.Tasks;

namespace TelegramBotHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var poller = new Poller("2078406572:AAFiM-IMgkbOwMNiN4nXiMevMxvdzAijtk8");
            await poller.Init();
        }
    }
}
