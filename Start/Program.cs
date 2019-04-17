using OverseerBot.Startup;
using SeymourBot.Startup;
using System;

namespace Start
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
