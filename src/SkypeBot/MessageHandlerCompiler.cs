using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            var handlers = new List<IMessageHandler>();

            var assemblyName = "SkypeBot.Handlers";

            var syntaxTrees = paths
                .Select(path => File.ReadAllText(path))
                .Select(contents => CSharpSyntaxTree.ParseText(contents))
                .ToArray();

            var dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Xml.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Xml.Linq.dll")),
                MetadataReference.CreateFromFile(typeof(Program).Assembly.Location), // SkypeBot
                MetadataReference.CreateFromFile(typeof(ILogger).Assembly.Location), // Serilog
            };

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, options);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    var assembly = Assembly.Load(ms.ToArray());
                    foreach (var handlerType in assembly.GetTypes().Where(t => !t.IsInterface && typeof(IMessageHandler).IsAssignableFrom(t)))
                    {
                        handlers.Add((IMessageHandler)Activator.CreateInstance(handlerType));
                        Log.Information("Initializing handler {HandlerType}", handlerType.Name);
                    }
                }
                else
                {
                    foreach (var error in result.Diagnostics)
                    {
                        var message = error.GetMessage();
                        var span = error.Location.GetLineSpan();
                        Log.Error("{FileName} line {LineNumber}: {ErrorMessage:l}", span.Path, span.StartLinePosition.Line, message);
                    }
                }
            }

            return handlers;
        }
    }
}
