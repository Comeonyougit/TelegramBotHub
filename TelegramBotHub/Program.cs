using System.Threading.Tasks;

namespace TelegramBotHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new FredagBot("");
            await bot.Start();
        }
    }
}
