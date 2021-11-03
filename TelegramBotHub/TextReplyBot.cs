using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotHub
{
    public abstract class TextReplyBot
    {
        private TelegramBotClient Client { get; set; }
        private int? NextUpdateId { get; set; }

        public string Name { get; }

        private Dictionary<string, Func<Update, Task>> CommandsDict { get; } = new Dictionary<string, Func<Update, Task>>();

        protected abstract List<(string name, string description, Func<Update, Task> func)> Commands { get; }

        private bool _firstPass = true;

        //private Timer _pollTimer = new Timer();

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
            var firstPass = _firstPass;
            _firstPass = false;
            if (updates == null || updates.Length == 0)
                return null;

            NextUpdateId = updates.Last().Id + 1;

            if (firstPass == true)
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
                    if (update.Type != UpdateType.Message)
                        return;

                    var text = update.Message?.Text;
                    var command = ParseCommand(update.Message?.Text);
                    if (CommandsDict.TryGetValue(command, out var botCommand))
                    {
                        await botCommand(update);
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
