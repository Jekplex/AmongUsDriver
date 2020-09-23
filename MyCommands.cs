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
        [Description("Moves everyone from the moderator's voice channel to a desired voice channel. (If you have spaces in your Voice Channel name please use quotation marks. For example: \"Among Us\")")]
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


    }
}
