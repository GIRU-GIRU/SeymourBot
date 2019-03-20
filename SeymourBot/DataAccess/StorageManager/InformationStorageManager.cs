using SeymourBot.DataAccess.Storage.Information;
using SeymourBot.Storage.Information;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.DataAccess.StorageManager
{
    class InformationStorageManager
    {
        public async Task StoreInfoCommandAsync(string CommandName, string CommandContent)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {

                    int commandID = GenerateCommandID(CommandName);

                    await db.InfoCommandTable.AddAsync(new InfoCommandTable
                    {
                        CommandID = commandID,
                        CommandName = CommandName,
                        CommandContent = CommandContent
                    });

                    await db.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetInfoCommand(string CommandName)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {

                   return db.InfoCommandTable.Where(x => 
                                                      x.CommandName.ToLower() == CommandName.ToLower())
                                                         .FirstOrDefault()
                                                             .CommandName;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private int GenerateCommandID(string commandName)
        {
            return 1;
        }
    }
}
