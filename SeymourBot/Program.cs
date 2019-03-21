using System;
using SeymourBot.Config;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using SeymourBot.Logging;
using SeymourBot.Exceptions;

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
        private SocketGuild guild;

        public async Task RunBotAsync()
        {
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


            _client.Log += Logger.Log;
            _commands.CommandExecuted += OnCommandExecutedAsync;

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


                //if (!Models.BlacklistUserStorage.BlackListedUser.Contains(context.Message.Author)) return;
                var result = await _commands.ExecuteAsync(context, argPos, _services);

            }
        }


        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            ITextChannel chnl = null;

            if (!string.IsNullOrEmpty(result.ErrorReason))
            {
                 chnl = await context.Guild.GetChannelAsync(558072353733738499) as ITextChannel;
            }
   
            switch (result)
            {
                case CommandErrorResult errorResult:
                    await chnl.SendMessageAsync(errorResult.Reason);
                    break;

                default:
                    if (!string.IsNullOrEmpty(result.ErrorReason))
                    {
                      await chnl.SendMessageAsync($"```{context.Message.Content}``` threw a {result.ErrorReason}");
                    }
                    break;
            }
        }




    }
}
