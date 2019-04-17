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

                cw("Launching Seymour..");
                var seymourBot = seymour.LaunchSeymourAsync();
                cw("Seymour Ready");

                cw("Launching Overseer..");
                var overseerBot = overseer.LaunchOverseerAsync();
                cw("Overseer ready..");

                seymourBot.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                Console.ReadLine();
            }

        }

        private static string cw(string input)
        {
            try
            {
                string timeStamp = $"{DateTime.Now.ToString("HH:mm:ss")}\t     ";
                return timeStamp + input;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
