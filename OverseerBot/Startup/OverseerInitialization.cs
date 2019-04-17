using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OverseerBot.UserMessageLogging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Config;

namespace OverseerBot.Startup
{
    public class OverseerInitialization
    {
        public async Task LaunchOverseerAsync()
        {
            await ConfigureOverseerAsync();

            var login = new OverseerLogIn();
            await login.RunBotAsync(_client);
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private async Task ConfigureOverseerAsync()
        {
            DiscordSocketConfig botConfig = new DiscordSocketConfig()
            {
                MessageCacheSize = ConfigManager.GetIntegerProperty(PropertyItem.MessageCacheSize)
            };

            var CommandServiceConfig = new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async
            };

            _client = new DiscordSocketClient(botConfig);
            _commands = new CommandService(CommandServiceConfig);
            _services = new ServiceCollection()
                .AddSingleton(_commands)
                .AddSingleton(_client)
                .BuildServiceProvider();

            _client.MessageUpdated += MessageLogger.EditedMessageEvent;
            _client.MessageDeleted += MessageLogger.DeletedMessageEvent;
        }     
    }
}
