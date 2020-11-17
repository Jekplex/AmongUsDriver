using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class MuteControlPanelCommands : BaseCommandModule
    {
        [Command("mutecp")]
        [Aliases("mcp")]
        public async Task MuteCP(CommandContext ctx, [RemainingText()] string extra)
        {

            var muteEmbed = new DiscordEmbedBuilder
            {
                Title = "Mute - Control Panel",
                Description = "React to this message to quickly mute and unmute players.",
                Color = DiscordColor.Orange,
            };

            // store tick and x
            var mute = ":mute:";
            var mute_emoji = DiscordEmoji.FromName(ctx.Client, mute);
            var x = ":x:";
            var x_emoji = DiscordEmoji.FromName(ctx.Client, x);

            // display text to the players
            muteEmbed.Description += Environment.NewLine + Environment.NewLine;
            muteEmbed.Description += $"{mute} - Mute / Unmute" + Environment.NewLine;
            muteEmbed.Description += $"{x} - Close" + Environment.NewLine;

            // display game embed
            var myMessage = await ctx.RespondAsync(embed: muteEmbed);

            // add tick reaction to embed.
            await myMessage.CreateReactionAsync(mute_emoji);
            await myMessage.CreateReactionAsync(x_emoji);

            // Wait to delete. Or delete when member who made game closes.
            await myMessage.WaitForReactionAsync(ctx.User, x_emoji, TimeSpan.FromHours(1f));

            // Delete all messages
            await myMessage.DeleteAsync();

            //await Task.CompletedTask;
        }

    }

}
