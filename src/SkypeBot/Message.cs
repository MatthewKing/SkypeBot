using System;

namespace SkypeBot
{
    public class Message
    {
        public DateTime TimeStamp { get; set; }
        public string ChatName { get; set; }
        public string Sender { get; set; }
        public string SenderDisplayName { get; set; }
        public string Text { get; set; }
    }
}
