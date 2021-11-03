using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBotHub
{
    public class FredagBot : TextReplyBot
    {
        public FredagBot(string accessToken)
            : base(accessToken)
        {
        }

        Random Random { get; } = new Random();

        protected override List<(string name, string description, Func<Update, Task> func)> Commands =>
            new List<(string name, string description, Func<Update, Task> func)>()
            {
                ("fredag", "is it fredag yet?!?!", PostFredag),
                ("droll", "roll some dice", Droll),
                ("gaydar", "is he tho?", Gaydar)
            };

        private Func<Update, Task> PostFredag
            => (x) =>
            {
                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday)
                    return PostText(x.Message.Chat, "Det ar fredag mina bekanta!");
                else
                    return PostText(x.Message.Chat, "Det ar not fredag mina bekanta");
            };

        private Func<Update, Task> Gaydar
            => (x) =>
            {
                if (Random.NextDouble() > 0.5f)
                    return PostText(x.Message.Chat, "Very gay");
                else
                    return PostText(x.Message.Chat, "Unconfirmed");
            };

        private Func<Update, Task> Droll
            => (x) =>
            {
                var match = DiceRegex.Match(x.Message?.Text);
                if (!match.Success)
                    return Task.CompletedTask;

                var first = Convert.ToInt32(match.Groups[1].Value);
                var second = Convert.ToInt32(match.Groups[2].Value);

                var sum = 0;

                foreach (var roll in Enumerable.Range(1, first))
                {
                    sum += Random.Next(1, second);
                }
                return PostText(x.Message.Chat, $"{match.Value} dice roll for @{x.Message.From.FirstName}: {sum}");
            };

        private Regex DiceRegex = new Regex(@"([1-9][0-9]|[1-9])[dD]([1-9][0-9]|[1-9])", RegexOptions.Compiled);
    }
}
