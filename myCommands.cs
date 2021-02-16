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
using DSharpPlus.VoiceNext;

namespace AmongUsDriver
{
    class myCommands : BaseCommandModule
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

            // Checks if user is in a voice channel on the server.
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
                return;
            }

            // If user is in a voice channel...

            // Start the muting process
            try
            {
                foreach (var member in ctx.Member.VoiceState.Channel.Users)
                {
                    await member.SetMuteAsync(true);
                }

                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":white_check_mark:"));

            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":x:"));
            }

        }
        
        [Command("unmute")]
        [Aliases("u")]
        [Description("Unmutes everyone in your current voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx)
        {

            // Checks if player is in a voice channel on the server.
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in a voice channel on this server.");
                return;
            }

            // If user is in a voice channel...

            // Start the unmuting process
            try
            {
                foreach (var member in ctx.Member.VoiceState.Channel.Users)
                {
                    await member.SetMuteAsync(false);
                }

                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":white_check_mark:"));
            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":x:"));
            }

        }

        [Command("refresh")]
        [Aliases("r", "reload")]
        [Description("Refreshes data. This needs to be done everytime someone new joins the vc and at the start of use. This ensures everyone get muted and unmuted.")]
        public async Task Refresh(CommandContext ctx)
        {
            await Program.discord.ReconnectAsync(true);

            await ctx.RespondAsync("Refreshing... Please wait a moment. (I won't tell you when I am ready. Worst-case scenario, I'll just ignore a few of your next commands.)");
        }


        [Command("move")]
        [Description("Moves everyone from your current voice channel to a desired voice channel.")]
        [RequirePermissions(DSharpPlus.Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, [RemainingText()] string voice_channel)
        {
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
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":white_check_mark:"));
            }
            catch
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly. (Or not everyone has permissions to access that room.)");
            }


        }

        [Command("clearmydms")]
        [Description("Clears the last 10 direct messages AUD has sent you.")]
        public async Task ClearMyDMs(CommandContext ctx)
        {
            var userDM = await ctx.Member.CreateDmChannelAsync();
            var messages = userDM.GetMessagesAsync(10).Result;
        
            foreach (var message in messages)
            {
                await message.DeleteAsync();
            }

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.discord, ":white_check_mark:"));
        }
                
    }

}
