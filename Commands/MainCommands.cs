using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AmongUsDriver.Commands
{
    public class MainCommands : BaseCommandModule
    {

        [Command("ping")]
        [Description("Returns 'Pong!'")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("pong!").ConfigureAwait(false);
        }

        // I want to restrict this bot to moderators.
        // Every moderator (in theory) should have the permission to mute players.

        [Command("mute")]
        [Description("Mutes everyone in your voice channel. (This server only)")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null)
            {
                await ctx.Channel.SendMessageAsync("You cannot be found in a voice channel in this server.").ConfigureAwait(false);
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                for (int i = 0; i < playerList.Count; i++)
                {
                    await playerList.ElementAt(i).SetMuteAsync(true).ConfigureAwait(false);
                }
                await ctx.Channel.SendMessageAsync("Players muted.").ConfigureAwait(false);
            }

        }

        [Command("unmute")]
        [Description("Unmutes everyone in the your voice channel. (This server only)")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null)
            {
                await ctx.Channel.SendMessageAsync("You cannot be found in a voice channel in this server.").ConfigureAwait(false);
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                for (int i = 0; i < playerList.Count; i++)
                {
                    await playerList.ElementAt(i).SetMuteAsync(false).ConfigureAwait(false);
                }
                await ctx.Channel.SendMessageAsync("Players unmuted.").ConfigureAwait(false);
            }

        }

        [Command("move")]
        [Description("Moves everyone from the moderator's voice channel to a desired voice channel. (If you have spaces in your Voice Channel name please use quotation marks. For example: \"Among Us\")")]
        [RequirePermissions(DSharpPlus.Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, string voice_channel_name)
        {

            // before doing these instructions check if player is even present a voice channel.
            if (ctx.Member.VoiceState == null)
            {
                await ctx.Channel.SendMessageAsync("You cannot be found in a voice channel in this server.").ConfigureAwait(false);
                return;
            }

            // get all voice channels
            var voiceChannelList = new List<DiscordChannel>();

            foreach (KeyValuePair<ulong, DiscordChannel> entry in ctx.Guild.Channels)
            {
                if (entry.Value.Type == ChannelType.Voice)
                {
                    voiceChannelList.Add(entry.Value);
                }
                else
                {
                    continue;
                }
            }

            // compare voice channels string with input string
            DiscordChannel targetVoiceChannel = ctx.Member.VoiceState.Channel;

            for (int i = 0; i < voiceChannelList.Count; i++)
            {
                if (voiceChannelList[i].Name == voice_channel_name)
                {
                    targetVoiceChannel = voiceChannelList[i];
                    break;
                }
                else
                {
                    continue;
                }
            }

            // if not found then return error else...

            if (targetVoiceChannel == ctx.Member.VoiceState.Channel)
            {
                await ctx.Channel.SendMessageAsync("Could not find the voice channel you want to join. Or you are already in that voice channel.");
                return;
            }

            // move all players from current room to that room.
            var membersInCurrentVoiceChannel = ctx.Member.VoiceState.Channel.Users.ToArray();

            for (int i = 0; i < membersInCurrentVoiceChannel.Length; i++)
            {
                await membersInCurrentVoiceChannel[i].PlaceInAsync(targetVoiceChannel).ConfigureAwait(false);
            }

        }

        //

        //private List<DiscordMember> gameQueue = new List<DiscordMember>();
        Dictionary<ulong, List<DiscordMember>> guildToQueue = new Dictionary<ulong, List<DiscordMember>>();
        List<DiscordMember> gameQueue = new List<DiscordMember>();

        [Command("join")]
        [Description("Adds you to the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.Speak)]
        public async Task Join(CommandContext ctx)
        {
            
            // If guild is in dictionary... then...
            if (guildToQueue.TryGetValue(ctx.Guild.Id, out gameQueue))
            {
                guildToQueue[ctx.Guild.Id].Add(ctx.Member);
            }
            else
            {
                gameQueue.Add(ctx.Member);
                guildToQueue.Add(ctx.Guild.Id, gameQueue);
                //guildToQueue[ctx.Guild.Id].Add(ctx.Member);
            }

            await ctx.Channel.SendMessageAsync(ctx.Member + " has been added to the game queue.");

            gameQueue.Clear();

            //List<DiscordMember> gameQueue = new List<DiscordMember>();



            // The person who executes this command gets added to the game queue.
            //gameQueue.Add(ctx.Member);
        }

        [Command("listdic")]
        [Description("Adds you to the game queue.")]
        [RequireOwner]
        public async Task ListDic(CommandContext ctx)
        {
            //await ctx.Channel.SendMessageAsync(guildToQueue.v);
            foreach (KeyValuePair<ulong, List<DiscordMember>> record in guildToQueue)
            {
                await ctx.Channel.SendMessageAsync(record.Key.ToString() + " " + record.Value.ToString());
            }

            //guildToQueue[ctx.Guild.Id].ToString(record.Key.ToString() + " " + record.Value.ToString());

            //List<DiscordMember> gameQueue = new List<DiscordMember>();

            //Program.integerList.Add(5);

            // The person who executes this command gets added to the game queue.
            //gameQueue.Add(ctx.Member);
        }

        [Command("leave")]
        [Description("Removes you to the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.Speak)]
        public async Task Leave(CommandContext ctx)
        {
            // The person who executes this command gets removed from the game queue.
            //

            for (int i = 0; i < guildToQueue[ctx.Guild.Id].Count; i++)
            {
                if (guildToQueue[ctx.Guild.Id][i].Id == ctx.Member.Id)
                {
                    guildToQueue[ctx.Guild.Id].RemoveAt(i);
                    await ctx.Channel.SendMessageAsync("You have been removed from the game queue.");
                    return;
                }
                else
                {
                    continue;
                }
            }

            await ctx.Channel.SendMessageAsync("You couldn't be found in the game queue.");

        }

        string theGameCode;

        [Command("setcodeandsend")]
        [Description("Sets the code and sends it out to people in the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task SetCodeAndSend(CommandContext ctx, string gamecode)
        {
            theGameCode = gamecode.ToUpper();

            // Sends a message to everyone in the queue.
            for (int i = 0; i < guildToQueue[ctx.Guild.Id].Count; i++)
            {
                await guildToQueue[ctx.Guild.Id][i].SendMessageAsync(
                    "A new Among Us lobby has been made!" + System.Environment.NewLine +
                    "The code: " + theGameCode.ToString()
                    );

            }

            await ctx.Channel.SendMessageAsync("Players have been notified.");

        }

        [Command("clearplayers")]
        [Description("Clears all players from the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task ClearPlayers(CommandContext ctx)
        {
            //gameQueue = new List<DiscordMember>();
            guildToQueue[ctx.Guild.Id].Clear();
            await ctx.Channel.SendMessageAsync("All players have been cleared from the game queue.");
        }

        [Command("list")]
        [Description("Lists all players in the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.Speak)]
        public async Task ListPlayers(CommandContext ctx)
        {
            if (guildToQueue[ctx.Guild.Id].Count <= 0)
            {
                await ctx.Channel.SendMessageAsync("There are no players in the queue.");
            }
            else
            {
                string output = ctx.Member.Mention + ", Here is the list of players in the queue:" + System.Environment.NewLine;

                for (int i = 0; i < guildToQueue[ctx.Guild.Id].Count; i++)
                {
                    if (i == guildToQueue[ctx.Guild.Id].Count - 1)
                    {
                        output += i + "    " + guildToQueue[ctx.Guild.Id][i].DisplayName;
                    }
                    else
                    {
                        output += i + "    " + guildToQueue[ctx.Guild.Id][i].DisplayName + System.Environment.NewLine;
                    }
                }
                await ctx.Channel.SendMessageAsync(output);

            }
        }

        [Command("kickid")]
        [Description("Kicks a player from the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task KickId(CommandContext ctx, int id)
        {
            if (guildToQueue[ctx.Guild.Id].Count == 0)
            {
                await ctx.Channel.SendMessageAsync("There are no players to kick in the queue.");
            }
            else
            {
                await ctx.Channel.SendMessageAsync(guildToQueue[ctx.Guild.Id][id].Mention + " has been removed from the game queue.");
                guildToQueue[ctx.Guild.Id].RemoveAt(id);
            }
        }

        [Command("kick")]
        [Description("Kicks a player from the game queue.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task ClearPlayers(CommandContext ctx, string player_name)
        {
            if (guildToQueue[ctx.Guild.Id].Count == 0)
            {
                await ctx.Channel.SendMessageAsync("There are no players to kick in the queue.");
            }
            else
            {
                for (int i = 0; i < guildToQueue[ctx.Guild.Id].Count; i++)
                {
                    if (guildToQueue[ctx.Guild.Id][i].DisplayName == player_name)
                    {
                        await ctx.Channel.SendMessageAsync(guildToQueue[ctx.Guild.Id][i].Mention + " has been removed from the game queue.");
                        guildToQueue[ctx.Guild.Id].RemoveAt(i);
                        return;
                    }
                    else
                    {
                        continue;
                    }

                }

                await ctx.Channel.SendMessageAsync("Could not find player name. Maybe try kickid instead?");
            }
        }









    }
}
