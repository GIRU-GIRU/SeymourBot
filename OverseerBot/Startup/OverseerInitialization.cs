﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OverseerBot.UserJoinedServer;
using OverseerBot.UserMessageLogging;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

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
                MessageCacheSize = ConfigManager.GetIntegerProperty(PropertyItem.MessageCacheSize),
                AlwaysDownloadUsers = true,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                LargeThreshold = 250,
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
            _client.MessageReceived += MessageLogger.ReceivedMessageEvent;
            _client.UserJoined += RoleApplication.ApplyPeasantRoleAsync;
            _client.Ready += InitializeContext;
        }

        private async Task InitializeContext()
        {
            try
            {
                DiscordContextOverseer.InitContext(_client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to intialize Overseer's discord context: ", ex.Message);
                throw;
            }
        }
    }
}
