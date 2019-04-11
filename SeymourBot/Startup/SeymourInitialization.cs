using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SeymourBot.AutoModeration;
using SeymourBot.Config;
using SeymourBot.DataAccess.Storage.Filter;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Logging;
using SeymourBot.Resources;
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
            _client.UserJoined += UserJoinedEvent;
            _client.MessageUpdated += MessageUpdatedEvent;

            await RegisterCommandAsync();
        }

        private async Task MessageUpdatedEvent(Cacheable<IMessage, ulong> oldMsg, SocketMessage newMsg, ISocketMessageChannel channel)
        {
            await MessageContentChecker.AutoModerateMessage(new SocketCommandContext(_client, newMsg as SocketUserMessage));
        }

        private async Task UserJoinedEvent(SocketGuildUser arg)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task BotReadyEvent()
        {
            DiscordContext.InitContext(_client);
            await BotStartupMessage();
        }
         

        public static async Task BotStartupMessage()
        {
            await DiscordContext.GetMainChannel().SendMessageAsync(BotDialogs.StartupMessage);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;
            var context = new SocketCommandContext(_client, message);

            await MessageContentChecker.AutoModerateMessage(context);

            int argPos = 0;
            if (message.HasStringPrefix(ConfigManager.GetProperty(PropertyItem.CommandPrefix), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
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

                    if (!string.IsNullOrEmpty(result.ErrorReason) && result.ErrorReason != "Unauthorized")
                    {
                        await DiscordContext.LogErrorAsync(result.ErrorReason, context.Message.Content);
                    }
                    break;
            }
        }
    }
}

