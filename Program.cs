using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmongUsDriver
{

    class Program
    {
        public static DiscordClient discord;

        static CommandsNextExtension commands;
        static InteractivityExtension interactivity;
        static VoiceNextExtension voiceNext;

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
                        
            // Bot Start
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            // Reads token and prefix from config.json located in project dir.
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            // Discord Config
            var discordConfig = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt"
            };
            discord = new DiscordClient(discordConfig);

            // Interactivity Config
            var interactivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            interactivity = discord.UseInteractivity(interactivityConfig);

            // Voice Next Config
            var voiceNextConfig = new VoiceNextConfiguration
            {
                EnableIncoming = false
            };
            voiceNext = discord.UseVoiceNext(voiceNextConfig);
            

            // Commands Config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                IgnoreExtraArguments = false,
            };
            commands = discord.UseCommandsNext(commandsConfig);

            // Linking myCommands
            commands.RegisterCommands<myCommands>(); 

            // In Event of a command error do this:
            commands.CommandErrored += Commands_CommandErrored;

            // When bot is ready...
            discord.Ready += Discord_Ready;

            //

            // BOT 'PLAYING...'
            DiscordActivity discordActivity = new DiscordActivity();
            discordActivity.ActivityType = ActivityType.Playing;
            discordActivity.Name = "Among Us | .help";

            // Connect and wait indefinitely.
            await discord.ConnectAsync(discordActivity);
            await Task.Delay(-1);
        }

        private static Task Discord_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Ready!");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Context.Message.Content[0].ToString() == "." && e.Context.Message.Content[1].ToString() == "." ||
                e.Context.Message.Content[0].ToString() == "." && e.Context.Message.Content[1].ToString() == "_")
            {
                // ignore
                await Task.CompletedTask;
            }
            else
            {
                await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error! - Stuck? Use '.help'");
            }

            

        }

    }
}
