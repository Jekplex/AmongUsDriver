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
    // THINKING
    // BOT VOICE .JOIN / .MUTE / .UNMUTE
    //

    class Program
    {
        public static DiscordClient discord; // DiscordClient or DiscordShardedClient?

        public static CommandsNextExtension commands;
        public static InteractivityExtension interactivity;
        public static VoiceNextExtension voiceNext;

        // My variables.
        public static Dictionary<ulong, bool> guildToBool_IsGameInProgress;
        public static Dictionary<ulong, string> guildToGameCode;

        //public static Dictionary<ulong, bool> guildToBool_IsMuteControlPanelOn;
        //public static Dictionary<ulong, DiscordUser> guildToMuteControlPanelUser;

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            // My Dictionaries
            guildToBool_IsGameInProgress = new Dictionary<ulong, bool>();
            guildToGameCode = new Dictionary<ulong, string>();
            
            //guildToBool_IsMuteControlPanelOn = new Dictionary<ulong, bool>();
            //guildToMuteControlPanelUser = new Dictionary<ulong, DiscordUser>();

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

            //
            var voiceConfig = new VoiceNextConfiguration
            {
                EnableIncoming = false
            };
            voiceNext = discord.UseVoiceNext(voiceConfig);
            

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
            //commands.RegisterCommands<SilentCommands>();
            //commands.RegisterCommands<MuteControlPanelCommands>();

            // In Event of a command error do this:
            commands.CommandErrored += Commands_CommandErrored;

            // On startup, when guilds become available - do this:
            discord.GuildAvailable += Discord_GuildAvailable;

            // When bot joins a guild...
            discord.GuildCreated += Discord_GuildCreated;

            // When bot leaves or is removed from a guild...
            discord.GuildDeleted += Discord_GuildDeleted;

            // When a reaction is added to any message...
            discord.MessageReactionAdded += Discord_MessageReactionAdded;

            // When a reaction is removed from any message...
            discord.MessageReactionRemoved += Discord_MessageReactionRemoved;

            // When bot is ready...
            discord.Ready += Discord_Ready;

            //

            // BOT 'LISTENING' 'PLAYING' 'STREAMING...
            DiscordActivity discordActivity = new DiscordActivity();
            discordActivity.ActivityType = ActivityType.Playing;
            discordActivity.Name = "Among Us | .help";

            // Connect and wait infinitely.
            await discord.ConnectAsync(discordActivity);
            await Task.Delay(-1);
        }

        private static Task Discord_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.WriteLine("Ready!");
            return Task.CompletedTask;
        }

        private static async Task Discord_MessageReactionRemoved(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionRemoveEventArgs e)
        {

            // Unmutes when user with permissions remove their reaction on MuteCP.
            //if
            //    (
            //    e.Message.Author == discord.CurrentUser &&
            //    e.Emoji == DiscordEmoji.FromName(discord, ":mute:") &&
            //    !e.User.IsBot &&
            //    (((DiscordMember)e.User).VoiceState != null || ((DiscordMember)e.User).VoiceState.Channel != null) &&
            //    /* ((DiscordMember)e.User).PermissionsIn(((DiscordMember)e.User).VoiceState.Channel).HasPermission(Permissions.MuteMembers) && */
            //    e.User == guildToMuteControlPanelUser[e.Guild.Id]
            //    )
            //{
            //
            //    string _out;
            //    var ctx = commands.CreateFakeContext(e.User, e.Channel, "su", ".", commands.FindCommand("su", out _out));
            //    await commands.ExecuteCommandAsync(ctx);
            //
            //}

            await Task.CompletedTask;

        }

        private static async Task Discord_MessageReactionAdded(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
        {

            // When people react to the SG game panel with a tick they get sent the code.
            
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
                        $"{Program.guildToGameCode[e.Guild.Id].ToUpper()}"
                    );
            }

            //

            // Mute when mute is reacted on MuteCP
            //if
            //    (
            //    e.Message.Author == discord.CurrentUser &&
            //    e.Emoji == DiscordEmoji.FromName(discord, ":mute:") &&
            //    !e.User.IsBot &&
            //    ( ((DiscordMember)e.User).VoiceState != null || ((DiscordMember)e.User).VoiceState.Channel != null ) &&
            //    /*((DiscordMember)e.User).PermissionsIn(((DiscordMember)e.User).VoiceState.Channel).HasPermission(Permissions.MuteMembers)*/
            //    e.User == guildToMuteControlPanelUser[e.Guild.Id]
            //    )
            //{
            //
            //    string _out;
            //    var ctx = commands.CreateFakeContext(e.User, e.Channel, "sm", ".", commands.FindCommand("sm", out _out));
            //    await commands.ExecuteCommandAsync(ctx);
            //    
            //}

        }

        private static async Task Discord_GuildDeleted(DiscordClient sender, DSharpPlus.EventArgs.GuildDeleteEventArgs e)
        {
            Console.WriteLine($">>> Left a guild: {e.Guild.Name}");
            
            Program.guildToBool_IsGameInProgress.Remove(e.Guild.Id);
            Program.guildToGameCode.Remove(e.Guild.Id);
            //Program.guildToBool_IsMuteControlPanelOn.Remove(e.Guild.Id);
            //Program.guildToMuteControlPanelUser.Remove(e.Guild.Id);
            
            await Task.CompletedTask;
        }

        private static async Task Discord_GuildCreated(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            Console.WriteLine($">>> Joined a new guild: {e.Guild.Name}");
            
            // Guild Setup
            Program.guildToBool_IsGameInProgress.Add(e.Guild.Id, false);
            Program.guildToGameCode.Add(e.Guild.Id, "");
            //Program.guildToBool_IsMuteControlPanelOn.Add(e.Guild.Id, false);
            //Program.guildToMuteControlPanelUser.Add(e.Guild.Id, null);

            await Task.CompletedTask;
        }

        private static async Task Discord_GuildAvailable(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            // Guild Setup
            Program.guildToBool_IsGameInProgress.Add(e.Guild.Id, false);
            Program.guildToGameCode.Add(e.Guild.Id, "");
            //Program.guildToBool_IsMuteControlPanelOn.Add(e.Guild.Id, false);
            //Program.guildToMuteControlPanelUser.Add(e.Guild.Id, null);

            await Task.CompletedTask;

        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {

            await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error! - Stuck? Use '.help'");
            //Console.WriteLine(e.Command.Name);

        }
    }
}
