using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

namespace SkypeBot
{
    public class MessageHandlerCompiler
    {
        public static IEnumerable<IMessageHandler> CompileHandlers(params string[] paths)
        {
            return CompileHandlers(paths as IEnumerable<string>);
        }

        public static IEnumerable<IMessageHandler> CompileHandlers(IEnumerable<string> paths)
        {
            var provider = new CSharpCodeProvider(new Dictionary<string, string>()
            {
                ["CompilerVersion"] = "v4.0"
            });

            var options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.ReferencedAssemblies.Add("SkypeBot.exe");

            var handlers = new List<IMessageHandler>();

            var results = provider.CompileAssemblyFromFile(options, paths.ToArray());
            if (results.Errors.Count == 0)
            {
                var assembly = results.CompiledAssembly;
                foreach (var handlerType in assembly.GetTypes().Where(t => !t.IsInterface && typeof(IMessageHandler).IsAssignableFrom(t)))
                {
                    handlers.Add((IMessageHandler)Activator.CreateInstance(handlerType));
                }
            }
            else
            {
                var err = results.Errors.OfType<CompilerError>().ToArray();
            }

            return handlers;
        }
    }
}
