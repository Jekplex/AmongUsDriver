using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmongUsDriver
{
    class Program
    {
        static DiscordClient discord;

        static CommandsNextExtension commands;

        public static Dictionary<ulong, List<DiscordMember>> guildToQueue;
        public static Dictionary<ulong, string> guildToCode;

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            // My Dictionaries
            guildToQueue = new Dictionary<ulong, List<DiscordMember>>();
            guildToCode = new Dictionary<ulong, string>();

            // Bot Start
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            // Reads token and prefix from config.json located in project dir.
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            // Discord Config
            var discordConfig = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,

                //UseInternalLogHandler = true,
                //LogLevel = LogLevel.Debug

                MinimumLogLevel = LogLevel.Debug,
            };
            discord = new DiscordClient(discordConfig);

            // Commands Config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                IgnoreExtraArguments = false,
            };
            commands = discord.UseCommandsNext(commandsConfig);

            // Linking MyCommands
            commands.RegisterCommands<MyCommands>();

            // In Event of a command error do this:
            commands.CommandErrored += async e =>
            {
                Console.WriteLine($"\"{e.Context.Message}\" Error! : {e.Exception.Message}");
                await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error! - Stuck? Use '.help'");
            };

            // On startup, when guilds become available - do this:
            discord.GuildAvailable += async e =>
            {
                Program.guildToQueue.Add(e.Guild.Id, new List<DiscordMember>());
                Program.guildToCode.Add(e.Guild.Id, "");

                await Task.CompletedTask;
            };

            // When bot joins a guild...
            discord.GuildCreated += async e =>
            {
                Console.WriteLine($"Joined a new guild: {e.Guild.Name}");

                // Guild Setup
                Program.guildToQueue.Add(e.Guild.Id, new List<DiscordMember>());
                Program.guildToCode.Add(e.Guild.Id, "");

                await Task.CompletedTask;
            };

            // When bot leaves or is removed from a guild...
            discord.GuildDeleted += async e =>
            {
                Console.WriteLine($"Left a guild: {e.Guild.Name}");

                Program.guildToQueue.Remove(e.Guild.Id);
                Program.guildToCode.Remove(e.Guild.Id);

                await Task.CompletedTask;
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}
