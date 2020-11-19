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
        [RequirePermissions(DSharpPlus.Permissions.MuteMembers)]
        public async Task MuteCP(CommandContext ctx, [RemainingText()] string extra)
        {
            //validation
            if (Program.guildToBool_IsMuteControlPanelOn[ctx.Guild.Id])
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, cannot create a mute control panel because one already exists.");
                return;
            }
            else
            {
                Program.guildToBool_IsMuteControlPanelOn[ctx.Guild.Id] = true;
            }

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

            // set user in
            Program.guildToMuteControlPanelUser[ctx.Guild.Id] = ctx.User;

            // display text to the players
            muteEmbed.Description += Environment.NewLine + Environment.NewLine;
            muteEmbed.Description += $"Control Panel User: {ctx.Member.Mention}" + Environment.NewLine;
            muteEmbed.Description += Environment.NewLine + Environment.NewLine;
            muteEmbed.Description += $"{mute} - Mute / Unmute" + Environment.NewLine;
            muteEmbed.Description += $"{x} - Close" + Environment.NewLine;

            // display game embed
            var myMessage = await ctx.RespondAsync(embed: muteEmbed);

            // add tick reaction to embed.
            await myMessage.CreateReactionAsync(mute_emoji);
            await myMessage.CreateReactionAsync(x_emoji);

            // Wait to delete. Or delete when member who made game closes.
            await myMessage.WaitForReactionAsync(ctx.User, x_emoji, TimeSpan.FromHours(3f));

            // Delete all messages
            await myMessage.DeleteAsync();
            Program.guildToBool_IsMuteControlPanelOn[ctx.Guild.Id] = false;
            Program.guildToMuteControlPanelUser[ctx.Guild.Id] = null;



        }

    }

}
