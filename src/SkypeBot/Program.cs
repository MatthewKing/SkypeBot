using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Skype = SKYPE4COMLib.Skype;

namespace SkypeBot
{
    public class Program
    {
        private class Context
        {
            public MessageReader Reader { get; set; }
            public MessageSender Sender { get; set; }
            public IMessageHandler[] Handlers { get; set; }
        }

        public static void Main(string[] args)
        {
            var skype = new Skype();
            skype.Attach();

            var database = FindSkypeDatabases().FirstOrDefault();

            var context = new Context();
            context.Reader = new MessageReader(database);
            context.Sender = new MessageSender(skype, "JovialBot");
            context.Handlers = MessageHandlerCompiler.CompileHandlers("Handlers.cs").ToArray();

            var timer = new Timer(Tick, context, 0, 3000);

            Console.ReadLine();
        }

        static void Tick(object state)
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

        private static IEnumerable<SkypeDatabase> FindSkypeDatabases()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var skypeDir = Path.Combine(appDataDir, "Skype");
            var dataFiles = Directory.GetFiles(skypeDir, "main.db", SearchOption.AllDirectories);
            return dataFiles.Select(path => new SkypeDatabase(path)).ToArray();
        }
    }
}
