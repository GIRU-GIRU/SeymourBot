using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SeymourBot.Config;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Startup
{
    class SeymourInitialization
    {
        public async Task LaunchSeymourAsync()
        {
            await ConfigureSeymourAsync();

            var login = new SeymourLogIn();
            await login.RunBotAsync(_client);
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private async Task ConfigureSeymourAsync()
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

            _client.Log += Logger.Log;
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _client.Ready += BotReadyEvent;

            await RegisterCommandAsync();
        }


        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }


        private async Task BotReadyEvent()
        {
            DiscordContext.InitContext(_client);
            await DiscordContext.BotReadyEvent();
        }



        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;

            // _ = Task.Run(() => _onMessage.MessageContainsAsync(arg)); //todo messagecontains checks, 
            int argPos = 0;
            if (message.HasStringPrefix(ConfigManager.GetProperty(PropertyItem.CommandPrefix), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                //todo blacklist check
                var result = await _commands.ExecuteAsync(context, argPos, _services);
            }
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            switch (result)
            {
                case CommandErrorResult errorResult:
                    await DiscordContext.LogError(result.ErrorReason);
                    break;

                default:
                    if (!string.IsNullOrEmpty(result.ErrorReason))
                    {
                        await DiscordContext.LogError(result.ErrorReason, context.Message.Content);
                    }
                    break;
            }
        }
    }
}

