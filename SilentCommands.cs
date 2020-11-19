using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class SilentCommands : BaseCommandModule
    {

        [Command("silentmute")]
        [Aliases("sm")]
        [Hidden()]
        [Description("Mutes everyone in your voice channel silently.")]
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
        [Description("Unmutes everyone in your voice channel silently.")]
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

    }
}
