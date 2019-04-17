using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OverseerBot.Logging;
using OverseerBot.Startup;
using OverseerBot.UserMessageLogging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                MessageCacheSize = 5000
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

           // _client.Log += ConsoleLogger.LogToConsoleTest;
           // _commands.CommandExecuted += OnCommandExecutedAsync;
           // _client.Ready += BotReadyEvent;
           // _client.UserJoined += UserJoinedEvent;
            _client.MessageUpdated += MessageLogger.EditedMessageEvent;
            _client.MessageDeleted += MessageLogger.DeletedMessageEvent;

            await RegisterCommandAsync();
        }


        private async Task UserJoinedEvent(SocketGuildUser arg)
        {
            throw new NotImplementedException(); // todo
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task BotReadyEvent()
        {
 
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;
            var context = new SocketCommandContext(_client, message);

            int argPos = 0;
            if (message.HasStringPrefix(".", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
            }
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            switch (result)
            {

                default:
                    break;
            }
        }
    }
}
