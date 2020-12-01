using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsDriver
{
    class FunCommands : BaseCommandModule
    {
        [Command("_.")]
        [Hidden()]
        public async Task LowFlatFace(CommandContext ctx, [RemainingText()] string extra)
        {
            await ctx.RespondAsync($"._. (me too)");
        }

        //[Command("peepeepoopo")]
        //[Hidden()]
        //public async Task PeepeePoopoo(CommandContext ctx, [RemainingText()] string extra)
        //{
        //    await ctx.RespondAsync($"YASSS {ctx.Member.Mention}  ");
        //}

    }

}
