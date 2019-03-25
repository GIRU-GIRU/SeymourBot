using SeymourBot.DataAccess.Storage.Information;
using SeymourBot.Exceptions;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
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
    static class StorageManager
    {
        public static async Task StoreInfoCommandAsync(Command command)
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
                ExceptionManager.HandleException("0601", ex);
            }
        }

        public static async Task<string> GetInfoCommand(Command command)
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
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0602", ex);
                throw;
            }
        }

        public static async Task StoreTimedEvent(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    if (db.UserStorageTable.FindAsync(newUser.UserID) == null)
                    {
                        //if the user id cannot be found, create it
                        //var oldUser = db.UserStorageTable.Where(x => x.UserName == newUser.UserName); no need to link with the name

                        await db.UserStorageTable.AddAsync(newUser);
                    }
                    await db.UserDisciplinaryEventStorageTable.AddAsync(newEvent);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0605", ex);
            }
        }

        public static async Task<List<string>> GetInfoCommands()
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
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0603", ex);
                throw;
            }
        }

        public static async Task<List<UserDisciplinaryEventStorage>> GetTimedEvents()
        {
            List<UserDisciplinaryEventStorage> result = new List<UserDisciplinaryEventStorage>();
            try
            {
                using (UserContext db = new UserContext())
                {
                    foreach (var element in db.UserDisciplinaryEventStorageTable)
                    {
                        result.Add(element);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0604", ex);
                throw;
            }
        }

        private static int GenerateCommandID(string commandName)
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
