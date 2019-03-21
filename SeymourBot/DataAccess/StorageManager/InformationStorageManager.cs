using SeymourBot.DataAccess.Storage.Information;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage.Information;
using SeymourBot.Storage.Information.Tables;
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
        public async Task StoreInfoCommandAsync(Command command)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {

                    int commandID = GenerateCommandID(command.CommandName);

                    await db.InfoCommandTable.AddAsync(new InfoCommandTable
                    {
                        CommandID = commandID,
                        CommandName = command.CommandName,
                        CommandContent = command.CommandContent
                    });

                    await db.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetInfoCommand(Command command)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {

                    return db.InfoCommandTable.Where(x =>
                                                       x.CommandName.ToLower() == command.CommandName.ToLower())
                                                          .FirstOrDefault()
                                                              .CommandName;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<string>> GetInfoCommands()
        {
            List<string> result = new List<string>();
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {
                    foreach (var element in db.InfoCommandTable)
                    {
                        result.Add(element.CommandName);
                    }
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private int GenerateCommandID(string commandName)
        {
            int id = 1;
            using (InfoCommandContext db = new InfoCommandContext())
            {
                id = db.InfoCommandTable.Count() + 1;
            }
            return id;
        }
    }
}
