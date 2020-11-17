using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class StandardCommands : BaseCommandModule
    {
        [Command("ping")]
        [Aliases("p")]
        [Description("Used to check if the bot is alive.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, pong!");
        }

        [Command("mute")]
        [Aliases("m")]
        [Description("Mutes everyone in your current voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {
                await ctx.RespondAsync(". . .");

                try
                {
                    foreach(var member in ctx.Member.VoiceState.Channel.Users)
                    {
                        await member.SetMuteAsync(true);
                    }
                    await ctx.RespondAsync($"{ctx.User.Mention}, All muted.");
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }


            }

        }

        
        [Command("unmute")]
        [Aliases("u")]
        [Description("Unmutes everyone in your current voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {

                await ctx.RespondAsync(". . .");

                try
                {
                    foreach (var member in ctx.Member.VoiceState.Channel.Users)
                    {
                        await member.SetMuteAsync(false);
                    }
                    await ctx.RespondAsync($"{ctx.User.Mention}, All unmuted.");
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }


            }

        }

        [Command("silentmute")]
        [Aliases("sm")]
        [Hidden()]
        [Description("Mutes everyone in your current voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task SilentMute(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {
                try
                {
                    foreach (var member in ctx.Member.VoiceState.Channel.Users)
                    {
                        await member.SetMuteAsync(true);
                    }
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }


            }

        }

        [Command("silentunmute")]
        [Aliases("su")]
        [Hidden()]
        [Description("Mutes everyone in your current voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task SilentUnmute(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
            }
            else
            {
                try
                {
                    foreach (var member in ctx.Member.VoiceState.Channel.Users)
                    {
                        await member.SetMuteAsync(false);
                    }
                }
                catch
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }
            }

        }

        [Command("move")]
        [Description("Moves everyone from your current voice channel to a desired voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, [RemainingText()] string voice_channel)
        {
            await ctx.RespondAsync(". . .");

            // before doing these instructions check if player is even present a voice channel.
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
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
                await ctx.RespondAsync($"{ctx.User.Mention}, I couldn't find the voice channel you wish to join. (Or you are already in that voice channel).");
                return;
            }

            // move all players from current room to that room.
            var membersInCurrentVoiceChannel = ctx.Member.VoiceState.Channel.Users.ToArray();

            try
            {
                foreach (var member in membersInCurrentVoiceChannel)
                {

                    await member.PlaceInAsync(targetVoiceChannel);

                }
                await ctx.RespondAsync($"{ctx.User.Mention}, All moved.");
            }
            catch
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly. (Or not everyone has permissions to access that room.)");
            }

        }

        [Command("cleardms")]
        [Description("Used to clear your dms with this bot. (Deletes max 10 messages per command.)")]
        public async Task ClearDMS(CommandContext ctx)
        {
            await ctx.RespondAsync($". . .");

            var userDM = await ctx.Member.CreateDmChannelAsync();
            var messages = userDM.GetMessagesAsync(10).Result.ToArray();
            if (messages.Length < 10)
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    await messages[i].DeleteAsync();
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    await messages[i].DeleteAsync();
                }
            }

            await ctx.RespondAsync($"{ctx.User.Mention} Deleted some dms.");
        }

        [Command("refresh")]
        [Aliases("r")]
        [Description("Refreshes the bot.")]
        public async Task Refresh(CommandContext ctx)
        {

            //await ctx.RespondAsync(". . .");
            //
            //await Program.discord.DisconnectAsync();
            //
            //// BOT 'LISTENING' 'PLAYING' 'STREAMING...
            //DiscordActivity discordActivity = new DiscordActivity();
            //discordActivity.ActivityType = ActivityType.Playing;
            //discordActivity.Name = "Among Us | .help";
            //
            //await Program.discord.ConnectAsync(discordActivity);
            //
            //
            //await ctx.RespondAsync("Refreshed.");

            await Program.discord.ReconnectAsync();
            await ctx.RespondAsync("Refreshed.");

        }

    }

}
