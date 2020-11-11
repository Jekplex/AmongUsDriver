# Among Us Driver (AUD)
An Among Us discord bot that allows you to mute and unmute players really quickly.

Origin
-----------

The idea for this discord bot occured to me when I was getting fustrated about people continuing to talk after the discussion phase in Among Us. It irritated me and I wanted a fix. So, I decided to make my own solution. I called it "Among Us Driver", AUD for short.
<!--
How to use
-----------
You can add my bot to your discord server by using the link below.

https://discord.com/api/oauth2/authorize?client_id=757258351146041474&permissions=57703504&scope=bot

Note: This link now has appropriate permissions for the current and potential future usage for this bot. If some permissions are not granted, the bot may not work as intended. Also the bot occasionally has development downtime as this is an ongoing project.

The bot's prefix is "."  
Use ".help" for commands. 
"Moderator" is the person executing the command.
-->
Features
-----------
* Able to mute and unmute all members in your voice channel via command.
* Able to move everyone in your current voice channel to another voice channel.
* Able to host Among Us games and distribute game code easily.

Contact Me
-----------
If you would like to contact me, you can reach me over at my discord server. https://discord.gg/WAV5v47

For Developers
-----------
In order to host your own version of this bot, there are some prerequisites. 

Step 0.5: You'll need a discord developer app and a bot attached.

You can make these in the Discord Developer Portal.

Step 1: You’ll need your own config.json file. So create one.

```json
{

    "prefix": ".",
    "token": "[Insert Discord Bot Token]"

}
```

Please replace insert your discord bot token where it says [Insert Discord Bot Token]. Here you can change the prefix if you would like.

Step 1.5: Simply be able to work in the .Net Core framework.

.Net Core 3.1 seems to work fine. So get that or greater.

Step 2: You’ll need to add some DSharpPlus packages from SlimGet (a nuget source). 

Packages you'll need:
* DSharpPlus
* DSharpPlus.CommandsNext
* DSharpPlus.Interactivity

You get these from SlimGet. You’ll need to add their URL to your NuGet sources. URL and more information can be found here: https://nuget.emzi0767.com/gallery/about

Now, you can add the three packages or just run. DOTNET should be able to grab the packages needed. :)
