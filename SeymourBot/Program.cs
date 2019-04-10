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

namespace SeymourBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new SeymourInitialization();

            try
            {
                var bot = start.LaunchSeymourAsync();
                bot.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                Console.ReadLine();
            }

        }
    }
}
