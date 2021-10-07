namespace TelegramBotHub
{
    public abstract class BaseBotCommand : IBotCommand
    {
        public BaseBotCommand()
        {
        }

        public virtual void Visit(string message)
        {
            if (ShouldHandle(message))
                Handle(message);
        }

        public abstract bool ShouldHandle(string message);

        public abstract void Handle(string message);
    }
}
