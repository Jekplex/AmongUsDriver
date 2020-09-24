using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class MyCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Used to check if bot is alive.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, pong!");
        }

        [Command("mute")]
        [Description("Mutes everyone in your voice channel. (This server only)")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                try
                {
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        await playerList.ElementAt(i).SetMuteAsync(true);
                    }
                    await ctx.RespondAsync($"{ctx.User.Mention}, all players have been muted.");
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }
                
            }

        }

        [Command("unmute")]
        [Description("Unmutes everyone in the your voice channel. (This server only)")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                try
                {
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        await playerList.ElementAt(i).SetMuteAsync(false);
                    }
                    await ctx.RespondAsync($"{ctx.User.Mention}, all players have been unmuted.");
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left/moved too quickly.");
                }
                
            }

        }

        [Command("move")]
        [Description("Moves everyone from the moderator's current voice channel to a desired voice channel. (If your target voice channel has spaces please use quotation marks. For example: \"Among Us\")")]
        [RequirePermissions(DSharpPlus.Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, string voice_channel)
        {

            // before doing these instructions check if player is even present a voice channel.
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel.");
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
                if (voiceChannelList[i].Name == voice_channel)
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
                await ctx.RespondAsync($"{ctx.User.Mention}, could not find the voice channel you wish to join. Or you are already in that voice channel.");
                return;
            }

            // move all players from current room to that room.
            var membersInCurrentVoiceChannel = ctx.Member.VoiceState.Channel.Users.ToArray();

            try
            {
                for (int i = 0; i < membersInCurrentVoiceChannel.Length; i++)
                {
                    await membersInCurrentVoiceChannel[i].PlaceInAsync(targetVoiceChannel).ConfigureAwait(false);
                }
            }
            catch
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left/moved too quickly.");
            }
            

        }

        // join
        [Command("join")]
        [Description("Adds you to the game queue.")]
        public async Task Join(CommandContext ctx)
        {
            if (Program.guildToQueue[ctx.Channel.GuildId].Contains(ctx.Member))
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you are already in the queue for this discord server.");
            }
            else
            {
                Program.guildToQueue[ctx.Channel.GuildId].Add(ctx.Member);
                await ctx.RespondAsync($"{ctx.User.Mention}, has been added to the queue.");
            }
        }

        // leave
        [Command("leave")]
        [Description("Removes you to the game queue.")]
        public async Task Leave(CommandContext ctx)
        {
            if (Program.guildToQueue[ctx.Channel.GuildId].Contains(ctx.Member))
            {
                Program.guildToQueue[ctx.Channel.GuildId].Remove(ctx.Member);
                await ctx.RespondAsync($"{ctx.User.Mention}, you have been removed from the queue.");
            }
            else
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in the queue.");
            }
        }

        // list
        [Command("list")]
        [Description("Shows the list of players in the game queue.")]
        public async Task List(CommandContext ctx)
        {
            // if queue is empty
            if (Program.guildToQueue[ctx.Channel.GuildId].Count == 0)
            {
                await ctx.RespondAsync("The queue is empty.");
            }
            else
            {
                string output = "Here's the list of players in the queue:" + System.Environment.NewLine;

                //output += "[ID - Name - HasSeat?]" + System.Environment.NewLine;

                for (int i = 0; i < Program.guildToQueue[ctx.Channel.GuildId].Count; i++)
                {
                    if (i < 10)
                    {
                        output += i.ToString() + " - " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + " - 👍" + System.Environment.NewLine;
                    }
                    else
                    {
                        output += i.ToString() + " - " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + " - 👎" + System.Environment.NewLine;
                    }
                }

                await ctx.RespondAsync(output);
            }

                

            
        }

        // setcodeandsend
        [Command("setcodeandsend")]
        [Description("Sets the game code and sends it to the first 10 people in the queue.")]
        public async Task SetCodeAndSend(CommandContext ctx, string code)
        {
            // set code
            Program.guildToCode[ctx.Channel.GuildId] = code;

            // send dms
            for (int i = 0; i < Program.guildToQueue[ctx.Channel.GuildId].Count; i++)
            {
                await Program.guildToQueue[ctx.Channel.GuildId][i].SendMessageAsync(
                    "--------------------" + System.Environment.NewLine +
                    ctx.Guild.Name + " / " + ctx.Member.DisplayName + System.Environment.NewLine + 
                    Program.guildToCode[ctx.Channel.GuildId].ToUpper());
            }
        }

        // clear
        [Command("clear")]
        [Description("Clears all players in the game queue.")]
        public async Task Clear(CommandContext ctx)
        {
            // grab guild playerlist then remove everyone from it.
            // ignore if playerlist is already empty.

            if (Program.guildToQueue[ctx.Channel.GuildId].Count == 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, the queue is already empty.");
            }
            else
            {
                Program.guildToQueue[ctx.Channel.GuildId].Clear();
                await ctx.RespondAsync($"{ctx.User.Mention}, the queue was cleared.");
            }

        }

        // kick
        [Command("kick")]
        [Description("Kicks a player by id from the game list.")]
        public async Task Kick(CommandContext ctx, int id)
        {
            
            if (id > Program.guildToQueue[ctx.Channel.GuildId].Count || id < 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! ID is out of bounds.");
                return;
            }
            else
            {
                Program.guildToQueue[ctx.Channel.GuildId].RemoveAt(id);
                await ctx.RespondAsync($"{Program.guildToQueue[ctx.Channel.GuildId].ElementAt(id).Mention} has been kicked from the queue.");
            }

        }

        [Command("test")]
        [RequireOwner]
        public async Task ClearDms(CommandContext ctx)
        {
            // i simply want this to clear old dms.

            //var channel = ctx.Client.GetChannelAsync(757988708115939360).Result;
            //var messages = channel.GetMessagesAsync(5).Result;
            //await channel.DeleteMessagesAsync(messages, "Waste");
            //await ctx.RespondAsync("test");

            //var channel = ctx.Client.GetChannelAsync(757988708115939360).Result;
            await ctx.RespondAsync("test" + ctx.Client.PrivateChannels);

            ;
            
        }

    }
}
