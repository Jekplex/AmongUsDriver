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
    class GameCommands : BaseCommandModule
    {

        [Command("startgame")]
        [Aliases("sg")]
        public async Task StartGame(CommandContext ctx/*,[RemainingText] string code*/)
        {
            // validation
            if (Program.guildToBool[ctx.Guild.Id]) // if guild active
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} A game already exists. Close the game by reacting: :x:");
                return;
            }

            // grab interactivity
            var interactivity = ctx.Client.GetInteractivity();

            // enable guild bool
            Program.guildToBool[ctx.Guild.Id] = true;

            // grab code
            var plsEnterCodeMessage = await ctx.RespondAsync($"{ctx.Member.Mention} Please enter code...");
            var a = await ctx.Channel.GetNextMessageAsync(x => x.Author == ctx.User);
            var codeMessage = a.Result;
            var code = codeMessage.Content;

            // delete user's message (code entered)
            await codeMessage.DeleteAsync();

            // add code to dictionary.
            Program.guildToCode[ctx.Guild.Id] = code;

            // notify user code is set.
            var codeSetMessage = await ctx.RespondAsync("Code set.");

            // create game embed
            var gameEmbed = new DiscordEmbedBuilder
            {
                Title = "Among Us",
                Description = "React to this message to get game code.",
                Color = DiscordColor.Orange,
            };

            // store tick and x
            var tick = ":white_check_mark:";
            var tick_emoji = DiscordEmoji.FromName(ctx.Client, tick);
            var x = ":x:";
            var x_emoji = DiscordEmoji.FromName(ctx.Client, x);

            // display text to the players
            gameEmbed.Description += Environment.NewLine + Environment.NewLine;
            gameEmbed.Description += $"{tick} - Ready" + Environment.NewLine;
            gameEmbed.Description += $"{x} - Cancel" + Environment.NewLine;

            // display game embed
            var myMessage = await ctx.RespondAsync(embed: gameEmbed);

            // add tick reaction to embed.
            await myMessage.CreateReactionAsync(tick_emoji);
            await myMessage.CreateReactionAsync(x_emoji);

            // Rest is handled by (Event Handler) Message Reaction Added.

            // Wait to delete message.
            await myMessage.WaitForReactionAsync(ctx.User, x_emoji, TimeSpan.FromHours(1f));
            
            // Delete all messages
            await myMessage.DeleteAsync();
            await codeSetMessage.DeleteAsync();
            await plsEnterCodeMessage.DeleteAsync();
            await ctx.Message.DeleteAsync();
            
            Program.guildToBool[ctx.Guild.Id] = false;

        }

    }
}
