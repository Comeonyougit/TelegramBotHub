using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotHub
{
    public abstract class TextReplyBot
    {
        private TelegramBotClient Client { get; set; }
        private int? NextUpdateId { get; set; }

        public string Name { get; }

        private Dictionary<string, Func<Update, string>> CommandsDict { get; } = new Dictionary<string, Func<Update, string>>();

        protected abstract List<(string name, string description, Func<Update, string> func)> Commands { get; }

        private bool _firstPass = true;

        public TextReplyBot(string accessToken)
        {
            Client = new TelegramBotClient(accessToken);
            RegisterCommands().Wait();
        }

        private async Task RegisterCommands()
        {
            var commandsList = new List<BotCommand>();

            foreach (var command in Commands)
            {
                CommandsDict.Add(command.name, command.func);
                commandsList.Add(new BotCommand() { Command = command.name, Description = command.description });
            }

            await Client.SetMyCommandsAsync(commandsList);
        }

        private async Task<Update[]> GetUpdates()
        {
            var updates = await Client.GetUpdatesAsync(offset: NextUpdateId ?? 0);
            if (updates == null || updates.Length == 0)
                return null;

            var oldId = NextUpdateId;
            NextUpdateId = updates.Last().Id + 1;

            if (oldId == null)
                updates = null;

            return updates;
        }

        public async Task Start(CancellationTokenSource ctx = null)
        {
            while (true)
            {
                await Task.Delay(1000);
                var updates = await GetUpdates();
                if (updates == null) continue;
                foreach (var update in updates)
                {
                    var text = update.Message?.Text;
                    var command = ParseCommand(update.Message?.Text);
                    if (CommandsDict.TryGetValue(command, out var botCommand))
                    {
                        await PostText(update.Message.Chat, botCommand(update));
                    }
                }
            }
        }

        public async Task PostText(Chat chat, string text)
        {
            await Client.SendTextMessageAsync(chat.Id, text);
        }

        private readonly Regex CommandRegex = new Regex(@"(?<=/).*(?=@)", RegexOptions.Compiled); 

        private string ParseCommand(string text)
        {
            return CommandRegex.Match(text)?.Value;
        }
    }
}
