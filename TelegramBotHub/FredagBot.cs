using System;
using System.Collections.Generic;
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

        protected override List<(string name, string description, Func<Update, string> func)> Commands =>
            new List<(string name, string description, Func<Update, string> func)>()
            {
                ("fredag", "is it fredag yet?!?!", PostFredag),
                ("notgayroll", "roll for not gays", RollNotGay)
            };

        private Func<Update, string> PostFredag
            => (x) =>
            {
                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday)
                    return "Det ar fredag mina bekanta!";
                else
                    return "Det ar not fredag mina bekanta";
            };

        private Func<Update, string> RollNotGay
            => (x) =>
            {
                var roll = Random.Next(100);
                return $"Not gay roll for @{x.Message.From.FirstName} {x.Message.From.LastName}: {roll}";
            };
    }
}
