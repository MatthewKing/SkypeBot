namespace SkypeBot
{
    public interface IMessageHandler
    {
        void Handle(MessageSender sender, Message message);
    }
}
