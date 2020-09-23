using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left too quickly.");
                }
                
            }

        }


    }
}
