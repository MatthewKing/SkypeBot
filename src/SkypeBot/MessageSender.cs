using System.Linq;
using SKYPE4COMLib;

namespace SkypeBot
{
    public class MessageSender
    {
        private readonly Skype skype;
        private readonly string name;

        public MessageSender(Skype skype, string name = null)
        {
            this.skype = skype;
            this.name = name;
        }

        public void SendMessage(string target, string message)
        {
            var chat = this.FindChat(target);
            if (chat != null)
            {
                if (name != null)
                {
                    message = $"[{name}]: {message}";
                }

                chat.SendMessage(message);
            }
        }

        private Chat FindChat(string target)
        {
            return this.skype.Chats
               .OfType<Chat>()
               .Where(c => c.Name == target)
               .FirstOrDefault();
        }
    }
}
