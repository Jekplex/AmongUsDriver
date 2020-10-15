# Among Us Driver
An Among Us discord bot that allows you to mute and unmute players really quickly.

Origin
-----------

The idea for this discord bot occured to me when I was getting fustrated about people continuing to talk after the discussion phase in Among Us. It irritated me and I wanted a fix. So, I decided to make a solution. I called it "Among Us Driver", AUD for short.

How to use
-----------
You can add my bot to your discord server by using the link below.

https://discord.com/api/oauth2/authorize?client_id=757258351146041474&permissions=57703504&scope=bot

Note: This link now has appropriate permissions for the current and potential future usage for this bot. If some permissions are not granted, the bot may not work as intended. Also the bot occasionally has development downtime as this is an ongoing project.

The bot's prefix is "."  
Use ".help" for commands. 
"Moderator" is the person executing the command.

Features
-----------
* A moderator can mute and unmute players via command.
* A moderator can move everyone in their current voice channel to another voice channel.
* Each discord server has a game queue.

Game queues allows members of the discord server to join a queue to play Among Us. Moderators can use ".set [code]" to set the game code of the current game lobby. Mods can combine this with ".send" to let the bot send a direct message (with the game code) to the first 10 members in the list. This is a streamer feature to ensure the game code doesn't get leaked mid-stream. 

Contact Me
-----------
If you would like to contact me, you can reach me over at my discord server. https://discord.gg/WAV5v47

For Developers
-----------
In order to host your own version of this bot, there are some prerequisites. 

Step 0.5: You'll need a discord developer app and a bot attached.

You can make these at in the Discord Developer Portal.

Step 1: You’ll need your own config.json file. So create one.

```json
{
    "token": "[Insert Discord Bot Token]",
    "token2": "[Insert Discord Bot 2 Token]",
    "prefix": "."
}
```

Please replace insert your discord bot token where it say [Insert Discord Bot Token].

Here you can change the prefix if you would like. Most people should leave “token2” blank. I’ve used it to be able to develop the bot on a different discord app while having the main bot still running and live for people to use. Note: To do this, you’ll have to change the “configJson.token” in Program.cs under Discord Config to “configJson.token2” (For advanced users). 

Step 1.5: Simply be able to work in the .Net framework.

.Net Core 3.1 seems to work fine. So get that or greater.

Step 2: You’ll need to add some DSharpPlus packages from SlimGet. 
(>= 4.0.0-nightly-00725) 

Packages you'll need:
* DSharpPlus
* DSharpPlus.CommandsNext
* DSharpPlus.Interactivity

**All packages need to be >= 4.0.0-nightly-00725.**

You get these from SlimGet. You’ll need to add their URL to your NuGet sources. URL can be found here: https://nuget.emzi0767.com/gallery/about

Now, you can add the three packages to the project and run. :)

