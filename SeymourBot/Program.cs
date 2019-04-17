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
using SeymourBot.DiscordUtilities;
using SeymourBot.Startup;
using OverseerBot.Startup;

namespace SeymourBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var seymour = new SeymourInitialization();
            var overseer = new OverseerInitialization();

            try
            {
                var seymourBot = seymour.LaunchSeymourAsync();
                var overseerBot = overseer.LaunchOverseerAsync();

                seymourBot.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                Console.ReadLine();
            }

        }
    }
}
