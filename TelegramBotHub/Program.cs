using System.Threading.Tasks;

namespace TelegramBotHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new FredagBot("2078406572:AAFiM-IMgkbOwMNiN4nXiMevMxvdzAijtk8");
            await bot.Start();
        }
    }
}
