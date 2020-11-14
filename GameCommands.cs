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
        public async Task StartGame(CommandContext ctx,[RemainingText] string code)
        {
            // validation
            if (code == null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} Cannot start a game without a game code.").ConfigureAwait(false);
                return;
            } 
            if (Program.guildToBool_IsGameInProgress[ctx.Guild.Id]) // if guild active
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} A game already exists. Close the game by reacting: :x:").ConfigureAwait(false);
                return;
            }

            // delete user's message (code entered)
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            // enable guild bool
            Program.guildToBool_IsGameInProgress[ctx.Guild.Id] = true;

            // add code to dictionary.
            Program.guildToCode[ctx.Guild.Id] = code;

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
            var myMessage = await ctx.RespondAsync(embed: gameEmbed).ConfigureAwait(false);

            // add tick reaction to embed.
            await myMessage.CreateReactionAsync(tick_emoji).ConfigureAwait(false);
            await myMessage.CreateReactionAsync(x_emoji).ConfigureAwait(false);

            // Rest is handled by (Event Handler) Message Reaction Added.

            // Wait to delete. Or delete when member who made game closes.
            await myMessage.WaitForReactionAsync(ctx.User, x_emoji, TimeSpan.FromHours(1f)).ConfigureAwait(false);
            
            // Delete all messages
            await myMessage.DeleteAsync().ConfigureAwait(false);
            Program.guildToBool_IsGameInProgress[ctx.Guild.Id] = false;

        }

    }
}
