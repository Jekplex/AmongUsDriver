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

namespace AmongUsDriver
{
    class MyCommands : BaseCommandModule
    {
        [Command("ping")]
        [Aliases("p")]
        [Description("Used to check if bot is alive.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, pong!");
        }

        [Command("mute")]
        [Aliases("m")]
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
        [Aliases("u")]
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
                    await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left/moved too quickly.");
                }
                
            }

        }

        [Command("move")]
        [Description("Moves everyone from the moderator's current voice channel to a desired voice channel. (If your target voice channel has spaces please use quotation marks. For example: \"Among Us\")")]
        [RequirePermissions(DSharpPlus.Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, [RemainingText()] string voice_channel) // need to test this.
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
                await ctx.RespondAsync($"{ctx.User.Mention}, I couldn't find the voice channel you wish to join. Or you are already in that voice channel.");
                return;
            }

            // move all players from current room to that room.
            var membersInCurrentVoiceChannel = ctx.Member.VoiceState.Channel.Users.ToArray();

            try
            {
                for (int i = 0; i < membersInCurrentVoiceChannel.Length; i++)
                {
                    await membersInCurrentVoiceChannel[i].PlaceInAsync(targetVoiceChannel).ConfigureAwait(false);
                }
            }
            catch
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! An individual left/moved too quickly.");
            }

        }

        /*
         * 
         * Below are a set of Game Queue Commands
         * 
         */



        // join
        [Command("join")]
        [Description("Adds you to the game queue.")]
        public async Task Join(CommandContext ctx)
        {
            if (Program.guildToQueue[ctx.Channel.GuildId].Contains(ctx.Member))
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you are already in the queue for this discord server.");
            }
            else
            {
                Program.guildToQueue[ctx.Channel.GuildId].Add(ctx.Member);
                await ctx.RespondAsync($"{ctx.User.Mention}, has been added to the queue.");
            }

        }

        // leave
        [Command("leave")]
        [Description("Removes you to the game queue.")]
        public async Task Leave(CommandContext ctx)
        {
            if (Program.guildToQueue[ctx.Channel.GuildId].Contains(ctx.Member))
            {
                Program.guildToQueue[ctx.Channel.GuildId].Remove(ctx.Member);
                await ctx.RespondAsync($"{ctx.User.Mention}, you have been removed from the queue.");
            }
            else
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you cannot be found in the queue.");
            }
        }

        // list
        [Command("list")]
        [Aliases("ls")]
        [Description("Shows the list of players in the game queue.")]
        public async Task List(CommandContext ctx)
        {
            var listEmbed = new DiscordEmbedBuilder
            {
                Title = "Game Queue",
                Description = "Here is the list of players in the queue:",
                Color = DiscordColor.Orange,
            };

            listEmbed.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);

            // if queue is empty
            if (Program.guildToQueue[ctx.Channel.GuildId].Count == 0)
            {
                listEmbed.Description += System.Environment.NewLine + System.Environment.NewLine + "The queue is empty.";
                await ctx.RespondAsync(embed: listEmbed);
            }
            else if (Program.guildToQueue[ctx.Channel.GuildId].Count <= 10)
            {
                listEmbed.AddField("Players that will recieve game code. 👍", "-", true);
                //listEmbed.AddField("Players that will NOT recieve game code. 👎", "-", true);

                //if (Program.guildToQueue[ctx.Channel.GuildId].Count > 10)
                //{
                //    listEmbed.Fields.ElementAt(0).Value = String.Empty;
                //    listEmbed.Fields.ElementAt(1).Value = String.Empty;
                //}
                //else
                //{
                //    listEmbed.Fields.ElementAt(0).Value = String.Empty;
                //}
                listEmbed.Fields.ElementAt(0).Value = String.Empty;

                for (int i = 0; i < Program.guildToQueue[ctx.Channel.GuildId].Count; i++)
                {
                    //if (i < 10)
                    //{
                    //    //listEmbed.Fields.ElementAt(0).Value += i.ToString() + " " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + System.Environment.NewLine;
                    //    listEmbed.Fields.ElementAt(0).Value += $"{i} {Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName}{System.Environment.NewLine}";
                    //}
                    //else
                    //{
                    //    //listEmbed.Fields.ElementAt(1).Value += i.ToString() + " " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + System.Environment.NewLine;
                    //    listEmbed.Fields.ElementAt(1).Value += $"{i} {Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName}{System.Environment.NewLine}";
                    //}
                    listEmbed.Fields.ElementAt(0).Value += $"{i+1}. {Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName}{System.Environment.NewLine}";
                }

                await ctx.RespondAsync(embed: listEmbed);
            }
            else
            {
                listEmbed.AddField("Players that will recieve game code. 👍", "-", true);
                listEmbed.AddField("Players that will NOT recieve game code. 👎", "-", true);

                listEmbed.Fields.ElementAt(0).Value = String.Empty;
                listEmbed.Fields.ElementAt(1).Value = String.Empty;

                for (int i = 0; i < Program.guildToQueue[ctx.Channel.GuildId].Count; i++)
                {
                    if (i < 10)
                    {
                        //listEmbed.Fields.ElementAt(0).Value += i.ToString() + " " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + System.Environment.NewLine;
                        listEmbed.Fields.ElementAt(0).Value += $"{i+1}. {Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName}{System.Environment.NewLine}";
                    }
                    else
                    {
                        //listEmbed.Fields.ElementAt(1).Value += i.ToString() + " " + Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName + System.Environment.NewLine;
                        listEmbed.Fields.ElementAt(1).Value += $"{i+1}. {Program.guildToQueue[ctx.Channel.GuildId].ElementAt(i).DisplayName}{System.Environment.NewLine}";
                    }
                }
            }


        }
        
        [Command("set")]
        [Description("Sets the game code for queue.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Set(CommandContext ctx,[RemainingText()] string code)
        {
            // set code
            Program.guildToCode[ctx.Channel.GuildId] = code; // adds code to guildToCode.

            // delete command message.
            await ctx.Message.DeleteAsync();
            await ctx.RespondAsync($"{ctx.Member.Mention}, your new code has been set.");

        }

        [Command("send")] // TODO - Modify to send specific participants a game code. 
        [Description("Sends the code out to players in the game queue.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Send(CommandContext ctx, int playerPosition = 0)
        {

            // grabs gamecode
            var code = Program.guildToCode[ctx.Channel.GuildId];

            if (code == "")
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, no code has been set.");
                return;
            }
            else
            {

                if (playerPosition == 0)
                {
                    // grab list of recipients
                    var playerList = Program.guildToQueue[ctx.Channel.GuildId];

                    // loop through playerlist, (cap at 10).
                    // save dm channel
                    var dm_channels = new List<DiscordDmChannel>();
                    // send message
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        if (i < 10)
                        {
                            dm_channels.Add(playerList.ElementAt(i).CreateDmChannelAsync().Result);
                            await playerList.ElementAt(i).SendMessageAsync(
                                ctx.Guild.Name + " / " + ctx.Member.DisplayName + System.Environment.NewLine +
                                Program.guildToCode[ctx.Channel.GuildId].ToUpper()
                                );
                        }
                        else
                        {
                            break;
                        }
                    }

                    await ctx.RespondAsync($"{ctx.Member.Mention}, code has been sent to the first 10 players.");

                    //wait for seconds
                    // minutes * seconds * milliseconds
                    var waitTime = 10 * 60 * 1000; // Currently 10 minutes
                    await Task.Delay(waitTime);

                    // delete messages
                    // loop through dm channels and messages.
                    for (int i = 0; i < dm_channels.Count(); i++)
                    {
                        //dm_channels[i].
                        var messages = dm_channels.ElementAt(i).GetMessagesAsync(2).Result;

                        for (int i2 = 0; i2 < messages.Count; i2++)
                        {
                            await messages[i2].DeleteAsync(); // deletes message
                        }
                    }
                }
                else
                {
                    var player = Program.guildToQueue[ctx.Channel.GuildId].ElementAt(playerPosition - 1);
                    var dm_channel = new List<DiscordDmChannel>();

                    dm_channel.Add(player.CreateDmChannelAsync().Result);
                    await player.SendMessageAsync
                        (ctx.Guild.Name + " / " + ctx.Member.DisplayName + System.Environment.NewLine + Program.guildToCode[ctx.Channel.GuildId].ToUpper());

                    await ctx.RespondAsync($"{ctx.Member.Mention}, code has been sent to {player.Mention}");


                    //wait for seconds
                    // minutes * seconds * milliseconds
                    var waitTime = 10 * 60 * 1000; // Currently 10 minutes
                    await Task.Delay(waitTime);

                    var messages = dm_channel.ElementAt(0).GetMessagesAsync(2).Result;
                    for (int i = 0; i < messages.Count; i++)
                    {
                        await messages[i].DeleteAsync();
                    }
                    
                }

                
            }

        }

        // clear
        [Command("clear")]
        [Description("Clears all players in the game queue.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Clear(CommandContext ctx)
        {
            // grab guild playerlist then remove everyone from it.
            // ignore if playerlist is already empty.

            if (Program.guildToQueue[ctx.Channel.GuildId].Count == 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, cannot clear queue as the queue is already empty.");
            }
            else
            {
                Program.guildToQueue[ctx.Channel.GuildId].Clear();
                await ctx.RespondAsync($"{ctx.User.Mention}, the queue is now empty.");
            }

        }

        // kick
        [Command("kick")]
        [Description("Kicks a player by position from the game queue.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Kick(CommandContext ctx, int pos)
        {
            
            //if (pos > Program.guildToQueue[ctx.Channel.GuildId].Count - 1 || pos < 0)
            //{
            //    await ctx.RespondAsync($"{ctx.User.Mention}, Error! Position is out of bounds.");
            //    return;
            //}
            //else
            //{
            //    var player = Program.guildToQueue[ctx.Channel.GuildId].ElementAt(pos).Mention;
            //    Program.guildToQueue[ctx.Channel.GuildId].RemoveAt(pos);
            //    await ctx.RespondAsync($"{player} has been kicked from the queue.");
            //}

            //

            if (pos > Program.guildToQueue[ctx.Channel.GuildId].Count || pos <= 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Error! Position is out of bounds.");
                return;
            }
            else
            {
                var player = Program.guildToQueue[ctx.Channel.GuildId].ElementAt(pos-1).Mention;
                Program.guildToQueue[ctx.Channel.GuildId].RemoveAt(pos-1);
                await ctx.RespondAsync($"{player} has been kicked from the queue.");
            }

        }

    }
}
