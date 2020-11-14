﻿using System;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmongUsDriver
{
    class Program
    {
        static DiscordClient discord; // DiscordClient or DiscordShardedClient?

        static CommandsNextExtension commands;

        static InteractivityExtension interactivity;

        public static Dictionary<ulong, bool> guildToBool_IsGameInProgress;
        public static Dictionary<ulong, string> guildToCode;

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            // My Dictionaries
            guildToBool_IsGameInProgress = new Dictionary<ulong, bool>();
            guildToCode = new Dictionary<ulong, string>();

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

                //UseInternalLogHandler = true,
                //LogLevel = LogLevel.Debug
                //MinimumLogLevel = LogLevel.Debug,
            };
            discord = new DiscordClient(discordConfig);

            // Interactivity Config
            var interactivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            interactivity = discord.UseInteractivity();

            // Commands Config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                IgnoreExtraArguments = false,
            };
            commands = discord.UseCommandsNext(commandsConfig);

            // Linking MyCommands
            commands.RegisterCommands<StandardCommands>();
            commands.RegisterCommands<FunCommands>();
            commands.RegisterCommands<GameCommands>();

            // In Event of a command error do this:
            commands.CommandErrored += Commands_CommandErrored;

            // On startup, when guilds become available - do this:
            discord.GuildAvailable += Discord_GuildAvailable;

            // When bot joins a guild...
            discord.GuildCreated += Discord_GuildCreated;

            // When bot leaves or is removed from a guild...
            discord.GuildDeleted += Discord_GuildDeleted;

            // When a reaction is added to any message...
            discord.MessageReactionAdded += Discord_MessageReactionAdded; ;
            
            // Connect and wait infinitely.
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task Discord_MessageReactionAdded(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
        {

            if
                (
                e.Message.Author == discord.CurrentUser &&
                e.Emoji == DiscordEmoji.FromName(discord, ":white_check_mark:") &&
                !e.User.IsBot &&
                guildToBool_IsGameInProgress[e.Guild.Id]
                )
            {

                var member = ((DiscordMember)e.User);

                await member.SendMessageAsync
                    (
                        $"From: {e.Guild.Name}\n" +
                        $"{Program.guildToCode[e.Guild.Id].ToUpper()}"
                    );
            }

        }

        private static async Task Discord_GuildDeleted(DiscordClient sender, DSharpPlus.EventArgs.GuildDeleteEventArgs e)
        {
            Console.WriteLine($"Left a guild: {e.Guild.Name}");
            
            Program.guildToBool_IsGameInProgress.Remove(e.Guild.Id);
            Program.guildToCode.Remove(e.Guild.Id);
            
            await Task.CompletedTask;
        }

        private static async Task Discord_GuildCreated(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            Console.WriteLine($"Joined a new guild: {e.Guild.Name}");
            
            // Guild Setup
            Program.guildToBool_IsGameInProgress.Add(e.Guild.Id, false);
            Program.guildToCode.Add(e.Guild.Id, "");
            
            await Task.CompletedTask;
        }

        private static async Task Discord_GuildAvailable(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            // Guild Setup
            Program.guildToBool_IsGameInProgress.Add(e.Guild.Id, false);
            Program.guildToCode.Add(e.Guild.Id, "");
            
            await Task.CompletedTask;

        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            //Console.WriteLine($"\"{e.Context.Message}\" Error! : {e.Exception.Message}");
            await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error! - Stuck? Use '.help'");

        }
    }
}
