SkypeBot
========

A customizable Skype bot that runs on the .NET framework.

Configuration file
------------------

SkypeBot looks for a `config.json` file in its directory. This file specifies the name of the bot, as well as the paths of any handlers to be compiled and added to the handler pipeline.

An example `config.json` is as follows:

```
{
    "BotName": "JovialBot",
    "HandlerFiles": [
        "Handlers\\EchoHandler.cs",
        "Handlers\\GreetingHandler.cs"
    ]
}
```

Handlers
--------

When SkypeBot is first launched, all handler files specified in `config.json` are compiled and added to the handler pipeline. Whenever a new message is received, it is run through all of the handlers in the pipeline.

An example handler is as follows:

```csharp
public class GreetingHandler : IMessageHandler
{
    public void Handle(MessageSender sender, Message message)
    {
        if (message.Text == "Hello")
        {
            sender.SendMessage(message.ChatName, "Hello, " + message.SenderDisplayName);
        }
    }
}
```

Additional information
----------------------

SkypeBot uses the Skype4COM library to send messages. As a result, Skype must be installed to both compile and run SkypeBot. For reference, the Skype4COM library is generally found at `C:\Program Files (x86)\Common Files\Skype\Skype4COM.dll`.

SkypeBot does NOT use the Skype4COM library to receive messages. While Skype4COM seems like the obvious way to receive messages, it is unfortunately unreliable, and often loses messages. To work around this, SkypeBot instead scans Skype's SQLite database to find any new messages.

Copyright
---------

Copyright 2015 Matthew King.

License
-------

SkypeBot is licensed under the [MIT License](http://opensource.org/licenses/MIT). Refer to license.txt for more information.
