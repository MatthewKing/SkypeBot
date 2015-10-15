using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Skype = SKYPE4COMLib.Skype;

namespace SkypeBot
{
    public class Program
    {
        private class Config
        {
            public string BotName { get; set; }
            public IList<string> HandlerFiles { get; private set; }

            public Config()
            {
                this.HandlerFiles = new List<string>();
            }
        }

        private class Context
        {
            public MessageReader Reader { get; set; }
            public MessageSender Sender { get; set; }
            public IMessageHandler[] Handlers { get; set; }
        }

        public static void Main(string[] args)
        {
            var config = GetConfig();

            var skype = new Skype();
            skype.Attach();

            var database = FindSkypeDatabases().FirstOrDefault();

            var context = new Context();
            context.Reader = new MessageReader(database);
            context.Sender = new MessageSender(skype, config.BotName);
            context.Handlers = MessageHandlerCompiler.CompileHandlers(config.HandlerFiles).ToArray();

            var timer = new Timer(Tick, context, 0, 3000);

            Console.ReadLine();
        }

        private static void Tick(object state)
        {
            var context = state as Context;
            if (context == null)
                return;

            var messages = context.Reader.GetUnreadMessages();
            foreach (var message in messages)
            {
                foreach (var handler in context.Handlers)
                {
                    handler.Handle(context.Sender, message);
                }
            }
        }

        private static Config GetConfig()
        {
            var path = "config.json";
            if (File.Exists(path))
            {
                var contents = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<Config>(contents);
                return config;
            }
            else
            {
                var config = new Config();
                return config;
            }
        }

        private static IEnumerable<SkypeDatabase> FindSkypeDatabases()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var skypeDir = Path.Combine(appDataDir, "Skype");
            var dataFiles = Directory.GetFiles(skypeDir, "main.db", SearchOption.AllDirectories);
            return dataFiles.Select(path => new SkypeDatabase(path)).ToArray();
        }
    }
}
