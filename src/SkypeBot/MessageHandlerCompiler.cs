using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Serilog;

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
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("System.Runtime.dll");
            options.ReferencedAssemblies.Add("Serilog.dll");

            var handlers = new List<IMessageHandler>();

            var results = provider.CompileAssemblyFromFile(options, paths.ToArray());
            if (results.Errors.Count == 0)
            {
                var assembly = results.CompiledAssembly;
                foreach (var handlerType in assembly.GetTypes().Where(t => !t.IsInterface && typeof(IMessageHandler).IsAssignableFrom(t)))
                {
                    handlers.Add((IMessageHandler)Activator.CreateInstance(handlerType));
                    Log.Information("Initializing handler {HandlerType}", handlerType.Name);
                }
            }
            else
            {
                foreach (var err in results.Errors.OfType<CompilerError>())
                {
                    Log.Error("{FileName} line {LineNumber}: {ErrorMessage:l}", Path.GetFileName(err.FileName), err.Line, err.ErrorText);
                }
            }

            return handlers;
        }
    }
}
