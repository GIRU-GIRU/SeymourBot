using System;
using SeymourBot.Config;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;

namespace SeymourBot
{
    class Program
    {
        private static ConfigManager cfgManager = new ConfigManager();

        static void Main(string[] args)
        {     
            var program = new Program();
            var bot = program.RunBotAsync();
            bot.Wait();
        }


        public DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {

            var test = cfgManager.GetProperty(PropertyItem.BotToken);
            DiscordSocketConfig botConfig = new DiscordSocketConfig()
            {
                MessageCacheSize = cfgManager.GetIntegerProperty(PropertyItem.MessageCacheSize)
            };

            _client = new DiscordSocketClient(botConfig);

            CommandServiceConfig CommandServiceConfig = new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async
            };
            _commands = new CommandService(CommandServiceConfig);
            _services = new ServiceCollection()
                .AddSingleton(_commands)
                .AddSingleton(_client)
                .BuildServiceProvider();

            _client.Log += Log;

            await RegisterCommandAsync();
            await _client.LoginAsync(TokenType.Bot, cfgManager.GetProperty(PropertyItem.BotToken));
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;

           // _ = Task.Run(() => _onMessage.MessageContainsAsync(arg));
            int argPos = 0;
            if (message.HasStringPrefix(cfgManager.GetProperty(PropertyItem.CommandPrefix), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                if (Models.BlacklistUser.BlackListedUser.Contains(context.Message.Author)) return;
                var result = await _commands.ExecuteAsync(context, argPos, _services);
            }
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
    }
}
