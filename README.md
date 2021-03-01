# Among Us Driver (AUD)
An Among Us discord bot that allows you to mute and unmute players really quickly.

Origin
-----------

The idea for this discord bot occured to me when I was getting fustrated about people continuing to talk after the discussion phase in Among Us. It irritated me and I wanted a fix. So, I decided to make my own solution. I called it "Among Us Driver", AUD for short.

Features
-----------
* Mute and unmute all members in your voice channel via command.
* Move all members in your current voice channel to another voice channel.

For Developers
-----------
In order to host your own version of this bot, there are some prerequisites. 

**Step 1:** You'll need a discord developer app and a bot attached.

**Step 2:** You’ll need to create your own config.json file.

```json
{

    "prefix": ".",
    "token": "[Insert Discord Bot Token]"

}
```

Replace the token string with your own discord bot token. Whilst you're here, you could change the bot's prefix.

**Step 3:** You need to be able to download and operate the .NET Core 3.1 framework.

**Step 4:** You’ll need to add a new NuGet source (SlimGet) to your Nuget sources.

You’ll need to add their URL to your NuGet sources. URL and more information can be found here: https://nuget.emzi0767.com/gallery/about

**Step 5:** Run.

With the newly added NuGet source, you should be able to run the program and the the framework should retrieve whatever it needs automatically. However, if it doesn't then you need to get the latest version of the following packages manually.

Packages you'll need:
* DSharpPlus
* DSharpPlus.CommandsNext
* DSharpPlus.Interactivity
* DSharpPlus.VoiceNext

Now you should be able to try Step 5 again.

**Happy hunting imposters!**
