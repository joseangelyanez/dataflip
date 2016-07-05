using Dataflip.Cli.ComponentModel;
using Dataflip.Cli.ComponentModel.CodeGenerators;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ConsoleColor forecolor = Console.ForegroundColor;
            ConsoleColor backgroundColor = Console.BackgroundColor;

            try
            {
                if (!File.Exists("dataflip.json"))
                {
                    HelpWithDataflipJsonFile();
                    return;
                }
                Console.Title = "Dataflip is working on some awesome stuff right now...";
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Working on it!");
                Console.WriteLine();
                GlobalProgress.Notify += GlobalProgress_Notify;

                string json = null;

                Console.WriteLine("Getting data from 'dataflip.json'.");
                using (var stream = File.Open("dataflip.json", FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                    json = reader.ReadToEnd();

                Console.WriteLine("File found, parsing configuration...");
                var config = GlobalConfiguration.FromJson(json);

                Console.WriteLine("Configuration looks OK. Resolving contexts...");
                foreach (var contextConfig in config.Contexts)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Working on {contextConfig.Name}, {contextConfig.Sprocs.Count} sproc(s)...");
                    Console.ForegroundColor = ConsoleColor.White;
                    ContextBuilder resolver = new ContextBuilder(contextConfig);
                    var context = resolver.BuildContext();
                    Console.WriteLine();
                    Console.WriteLine("Generating code...");
                    CSharpContextGenerator generator = new CSharpContextGenerator(context);
                    string code = generator.GenerateCode();

                    TypeScriptTypingsGenerator typeScript = new TypeScriptTypingsGenerator(context);
                    string typeScriptCode = typeScript.GenerateCode();

                    HtmlBindingsGenerator htmlBindings = new HtmlBindingsGenerator(context);
                    string htmlBindingsCode = htmlBindings.GenerateCode();

                    Console.WriteLine($"Writing code to {contextConfig.Output}...");
                    using (FileStream stream = File.Create(contextConfig.Output))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(code);
                        Console.WriteLine();
                        Console.WriteLine("Awesome! C# Context class written successfully.");
                    }

                    if (contextConfig.TypeScriptTypings != null)
                    {
                        Console.WriteLine($"Working on typescript typings! Creating file {contextConfig.TypeScriptTypings}...");
                        using (FileStream stream = File.Create(contextConfig.TypeScriptTypings))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(typeScriptCode);
                            Console.WriteLine();
                            Console.WriteLine("Great! TypeScript typings file written successfully.");
                        }
                    }

                    if (contextConfig.AngularHtmlBindings != null)
                    {
                        Console.WriteLine($"Working on angular html bindings! Creating file {contextConfig.AngularHtmlBindings}...");
                        using (FileStream stream = File.Create(contextConfig.AngularHtmlBindings))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(htmlBindingsCode);
                            Console.WriteLine();
                            Console.WriteLine("Excellent! TypeScript typings file written successfully.");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Title = "Darn... I guess not.";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Ouch!");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Write any line to continue...");
                Console.Read();
            }

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = forecolor;
        }

        /// <summary>
        /// Helps create a new dataflip.json file.
        /// </summary>
        private static void HelpWithDataflipJsonFile()
        {
            Console.WriteLine("Thanks for using Dataflip!");
            Console.WriteLine();
            Console.WriteLine("A dataflip.json hasn't been created yet, do you want dataflip to create one for you? (It will be created with some guidelines on how to get started) [Y/N]");
            if (Console.ReadKey().KeyChar.ToString().ToUpper() == "Y")
            {
                using (var writer = File.CreateText("dataflip.json"))
                    writer.Write(Dataflip.Cli.DefaultFiles.DefaultDataflipJson);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Great! The file has been created with some demo configuration that you can tweak to start using Dataflip.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Questions? For more information go to ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("http://www.getdataflip.com");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.Read();
            }
        }

        /// <summary>
        /// Renders the notification on the Console window.
        /// </summary>
        /// <param name="obj">The object that contains the notification.</param>
        private static void GlobalProgress_Notify(GlobalProgressNotification obj)
        {
            Console.ForegroundColor = obj.Color;
            Console.WriteLine(obj.Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
