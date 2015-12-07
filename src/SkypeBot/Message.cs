using System;

namespace SkypeBot
{
    public class Message
    {
        private DateTime timeStamp;
        public DateTime Timestamp
        {
            get { return this.timeStamp; }
            set { this.timeStamp = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }

        private string chatName;
        public string ChatName
        {
            get { return this.chatName; }
            set { this.chatName = value; }
        }

        private string sender;
        public string Sender
        {
            get { return this.sender; }
            set { this.sender = value; }
        }

        private string senderDisplayName;
        public string SenderDisplayName
        {
            get { return this.senderDisplayName; }
            set { this.senderDisplayName = value; }
        }

        private string text;
        public string Text
        {
            get { return this.text; }
            set { this.text = FormatText(value); }
        }

        private static string FormatText(string text)
        {
            return text.Replace("&lt;", "<")
                       .Replace("&amp;", "&")
                       .Replace("&gt;", ">")
                       .Replace("&quot;", "\"")
                       .Replace("&apos;", "'");
        }
    }
}
