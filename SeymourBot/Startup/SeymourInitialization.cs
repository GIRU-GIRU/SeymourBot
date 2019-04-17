using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SeymourBot.AutoModeration;
using SeymourBot.Modules.DisciplinaryCommands;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Startup
{
    public class SeymourInitialization
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

            _client.Log += Toolbox.Logging.Logger.Log;
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _client.UserJoined += UserJoinedChecker.SanitizeJoinedUser;
            _client.MessageUpdated += MessageUpdatedEvent;

            await RegisterCommandAsync();
        }

        private async Task MessageUpdatedEvent(Cacheable<IMessage, ulong> oldMsg, SocketMessage newMsg, ISocketMessageChannel channel)
        {
            await MessageContentChecker.AutoModerateMessage(new SocketCommandContext(_client, newMsg as SocketUserMessage));
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(typeof(Confinement).Assembly, _services);
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

