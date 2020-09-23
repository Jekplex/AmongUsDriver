using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmongUsDriver
{
    class Program
    {
        static DiscordClient discord;

        static CommandsNextExtension commands;

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            // Reads token and prefix from config.json located in project dir.
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            // Discord Config
            var discordConfig = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,

                //UseInternalLogHandler = true,
                //LogLevel = LogLevel.Debug
                MinimumLogLevel = LogLevel.Debug,
            };

            discord = new DiscordClient(discordConfig);

            // Setting up commands config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                IgnoreExtraArguments = false,
            };

            // Setting up commands
            commands = discord.UseCommandsNext(commandsConfig);
            commands.RegisterCommands<MyCommands>();

            // Hooking into the event message created
            //discord.MessageCreated += async e =>
            //{
            //    if (e.Message.Content.ToLower().StartsWith("ping"))
            //        await e.Message.RespondAsync("pong!");
            //};

            // In Event CommandError do this...
            commands.CommandErrored += async e =>
            {
                await e.Context.RespondAsync($"{e.Context.Member.Mention}, Command Error!");
            };

            await discord.ConnectAsync();

            await Task.Delay(-1);

        }

    }
}
