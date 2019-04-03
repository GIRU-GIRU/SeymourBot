using Microsoft.EntityFrameworkCore;
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
                    await db.InfoCommandTable.AddAsync(new InfoCommandTable
                    {
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

        public static async Task<string> GetInfoCommandAsync(Command command)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {
                    var result = await db.InfoCommandTable.Where(x =>
                                                        x.CommandName.ToLower() == command.CommandName.ToLower())
                                                           .FirstOrDefaultAsync();


                    return result.CommandName;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0602", ex);
                throw;
            }
        }

        public static async Task<ulong> StoreTimedEventAsync(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var findResult = await db.UserStorageTable.FindAsync(newUser.UserID);
                    if (findResult == null)
                    {
                        await db.UserStorageTable.AddAsync(newUser);
                    }
                    await db.UserDisciplinaryEventStorageTable.AddAsync(newEvent);
                    await db.SaveChangesAsync();
                    return newEvent.DisciplineEventID;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0605", ex);
                throw;
            }
        }

        public static async Task ArchiveTimedEventAsync(ulong eventId)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var eventToArchive = await db.UserDisciplinaryEventStorageTable.FindAsync(eventId);
                    if (eventToArchive != null)
                    {
                        db.UserDisciplinaryEventStorageTable.Remove(eventToArchive);
                        var archivedEvent = new UserDisciplinaryEventArchive()
                        {
                            DateArchived = DateTime.Now,
                            DateInserted = eventToArchive.DateInserted,
                            DateToRemove = eventToArchive.DateToRemove,
                            DisciplineEventID = eventToArchive.DisciplineEventID,
                            DisciplineType = eventToArchive.DiscipinaryEventType,
                            ModeratorID = eventToArchive.ModeratorID,
                            Reason = eventToArchive.Reason,
                            UserID = eventToArchive.UserID
                        };
                        await db.UserDisciplinaryEventArchiveTable.AddAsync(archivedEvent);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        ExceptionManager.HandleException("0606", new Exception());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0606", ex);
            }
        }

        public static List<string> GetInfoCommands()
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

        public static async Task StoreDisciplinaryEventAsync(UserDisciplinaryEventStorage obj)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    await db.UserDisciplinaryEventStorageTable.AddAsync(obj);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0604", ex); //todo
                throw;
            }
        }

        public static async Task<int> GetRecentWarningsAsync(ulong userID)
        {
            try
            {
                using (UserContext db = new UserContext())
                {

                    //todo datetimenow.adddays configurable
                    int warnCount = await db.UserDisciplinaryEventStorageTable.Where(x => x.DiscipinaryEventType == DisciplinaryEventEnum.WarnEvent)
                                                                                .Where(x => x.UserID == userID)
                                                                                    .Where(x => x.DateToRemove <= DateTime.Now.AddDays(14))
                                                                                        .CountAsync();
                    return warnCount;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0604", ex); //todo
                throw;
            }
        }
    }
}
