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
                Token = configJson.Token2,
                TokenType = TokenType.Bot,

                //UseInternalLogHandler = true,
                //LogLevel = LogLevel.Debug
                MinimumLogLevel = LogLevel.Debug,
            };

            discord = new DiscordClient(discordConfig);

            // Setting up commands config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                IgnoreExtraArguments = false,
            };

            // Setting up commands
            commands = discord.UseCommandsNext(commandsConfig);
            commands.RegisterCommands<MyCommands>();

            // Hooking into the event message created
            //discord.MessageCreated += async e =>
            //{
            //    if (e.Message.Content.ToLower().StartsWith("ping"))
            //        await e.Message.RespondAsync("pong!");
            //};

            // When ready grab all discord servers and add them to the guildToQueue dictionary
            discord.Ready += async e =>
            {
                //e.Client.Guilds[]
                //foreach (KeyValuePair<ulong, DiscordGuild> entry in e.Client.Guilds)
                //{
                //    //entry.Value.a
                //}
                
                var guildList = e.Client.Guilds.ToList();

                for (int i = 0; i < guildList.Count; i++)
                {
                    guildToQueue.Add(guildList.ElementAt(i).Key, new List<DiscordMember>());
                    guildToCode.Add(guildList.ElementAt(i).Key, "");
                }

                await Task.CompletedTask;

            };

            // In Event of CommandError do this...
            commands.CommandErrored += async e =>
            {
                await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error! - Stuck? Use '.help'");
            };

            await discord.ConnectAsync();

            await Task.Delay(-1);

        }

    }
}
