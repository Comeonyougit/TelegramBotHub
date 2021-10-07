namespace TelegramBotHub
{
    public interface IBotCommand
    {
        public bool ShouldHandle(string message);

        public void Handle(string message);
    }
}
