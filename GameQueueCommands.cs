using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class GameQueueCommands : BaseCommandModule
    {

        [Command("startgame")]
        [Aliases("sg")]
        public async Task StartGame(CommandContext ctx/*,[RemainingText] string code*/)
        {

            if (Program.guildToBool[ctx.Guild.Id]) // if active
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}. Use .cg to stop current game.");
                return;
            }

            // grab interactivity
            var interactivity = ctx.Client.GetInteractivity();

            Program.guildToBool[ctx.Guild.Id] = true;

            var plsEnterCodeMessage = await ctx.RespondAsync($"{ctx.Member.Mention} Please enter code...");

            var a = await ctx.Channel.GetNextMessageAsync(x => x.Author == ctx.User);
            var codeMessage = a.Result;
            var code = codeMessage.Content;

            // delete user's message
            await codeMessage.DeleteAsync();

            // add code to dictionary.
            Program.guildToCode[ctx.Guild.Id] = code;

            //await plsEnterCodeMessage.ModifyAsync("Code set.");
            var codeSetMessage = await ctx.RespondAsync("Code set.");

            // create embed
            var gameEmbed = new DiscordEmbedBuilder
            {
                Title = "Among Us",
                Description = "React to this message to get game code.",
                Color = DiscordColor.Orange,
            };

            // store tick/x
            var tick = ":white_check_mark:";
            var DiscordEmoji_tick = DiscordEmoji.FromName(ctx.Client, tick);

            // Display text to the players
            gameEmbed.Description += Environment.NewLine + Environment.NewLine;
            gameEmbed.Description += $"{tick} - Ready" + Environment.NewLine;
            gameEmbed.Description += $":x: - Close" + Environment.NewLine;

            // Send embed
            var myMessage = await ctx.RespondAsync(embed: gameEmbed);

            // Add tick reaction to embed.
            await myMessage.CreateReactionAsync(DiscordEmoji_tick);
            await myMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));

            // Rest is handled by (Event Handler) Message Reaction Added.

            // Wait to delete message.
            await myMessage.WaitForReactionAsync(ctx.User, DiscordEmoji.FromName(ctx.Client, ":x:"), TimeSpan.FromMinutes(5f));
            await myMessage.DeleteAsync();
            await plsEnterCodeMessage.DeleteAsync();
            await codeSetMessage.DeleteAsync();
            Program.guildToBool[ctx.Guild.Id] = false;


        }

        [Command("stopgame")]
        [Aliases("cg")]
        public async Task StopGame(CommandContext ctx)
        {
            Program.guildToBool[ctx.Guild.Id] = false;
            await ctx.RespondAsync($"{ctx.Member.Mention} Success! You can now use .startgame or .sg");
        }

    }
}
