using OverseerBot.Startup;
using SeymourBot.Startup;
using System;

namespace Start
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var seymour = new SeymourInitialization();
                var overseer = new OverseerInitialization();
                string timeStamp = $"{DateTime.Now.ToString("HH:mm:ss")}\t     ";

                Console.WriteLine($"{timeStamp}Launching Seymour..");
                var seymourBot = seymour.LaunchSeymourAsync();
                Console.WriteLine($"{timeStamp}Seymour Ready");

                Console.WriteLine($"{timeStamp}Launching Overseer..");
                var overseerBot = overseer.LaunchOverseerAsync();
                Console.WriteLine($"{timeStamp}Overseer ready..");

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
