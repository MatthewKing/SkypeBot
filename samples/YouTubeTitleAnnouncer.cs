using System;
using System.Net;
using System.Text.RegularExpressions;
using SkypeBot;

public class YouTubeTitleAnnouncer : IMessageHandler
{
    public void Handle(MessageSender sender, Message message)
    {
        if (message.Text.IndexOf("youtube") == -1)
            return;

        var clipId = GetClipId(message.Text);
        if (clipId == null)
            return;

        var url = String.Format("https://www.youtube.com/watch?v={0}", clipId);
        var content = GetPageContents(url);
        if (content == null)
            return;

        var title = GetTitle(content);
        if (title == null)
            return;

        sender.SendMessage(message.ChatName, title);
    }

    private static string GetClipId(string url)
    {
        var match = Regex.Match(url, "https?:\\/\\/www\\.youtube\\.com\\/watch.*?v=([a-zA-Z0-9\\-_]+).*");
        return match.Success
            ? match.Groups[1].Value
            : null;
    }

    private static string GetTitle(string content)
    {
        var match = Regex.Match(content, "<title>(.*?)</title>");
        return match.Success
            ? match.Groups[1].Value.Replace(" - YouTube", null)
            : null;
    }

    private static string GetPageContents(string url)
    {
        using (var client = new WebClient())
        {
            return client.DownloadString(url);
        }
    }
}
