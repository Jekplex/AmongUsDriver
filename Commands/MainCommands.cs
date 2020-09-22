using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver.Commands
{
    public class MainCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns 'Pong!'")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
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
                await ctx.Channel.SendMessageAsync("You cannot be found in a voice channel.").ConfigureAwait(false);
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                for (int i = 0; i < playerList.Count; i++)
                {
                    await playerList.ElementAt(i).SetMuteAsync(true).ConfigureAwait(false);
                }
                await ctx.Channel.SendMessageAsync("Done! Everyone has been muted.").ConfigureAwait(false);
            }

        }

        [Command("unmute")]
        [Description("Unmutes everyone in the your voice channel. (This server only)")]
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)] 
        public async Task Unmute(CommandContext ctx)
        {

            if (ctx.Member.VoiceState == null)
            {
                await ctx.Channel.SendMessageAsync("You cannot be found in a voice channel.").ConfigureAwait(false);
            }
            else
            {
                var playerList = ctx.Member.VoiceState.Channel.Users.ToList();

                for (int i = 0; i < playerList.Count; i++)
                {
                    await playerList.ElementAt(i).SetMuteAsync(false).ConfigureAwait(false);
                }
                await ctx.Channel.SendMessageAsync("Done! Everyone has been unmuted.").ConfigureAwait(false);
            }

        }

    }
}
