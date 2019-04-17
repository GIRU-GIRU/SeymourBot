using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SeymourBot.AutoModeration;
using SeymourBot.DataAccess.Storage.Filter;
using SeymourBot.DataAccess.Storage.Filter.Tables;
using SeymourBot.DataAccess.Storage.Information;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.Information.Tables;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.Exceptions;
using Toolbox.Resources;

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
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
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
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
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
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
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
                            DateArchived = DateTime.UtcNow,
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
                        ExceptionManager.HandleException(ErrMessages.StorageException, new Exception());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
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
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
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
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task StoreDisciplinaryPermanentEventAsync(UserDisciplinaryPermanentStorage obj, UserStorage user)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    if (!await db.UserStorageTable.AnyAsync(x => x.UserID == user.UserID))
                    {
                        await db.AddAsync(user);
                    }

                    await db.UserDisciplinaryPermanentStorageTable.AddAsync(obj);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<int> GetRecentWarningsAsync(ulong userID)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    int warnCount = await db.UserDisciplinaryEventStorageTable.Where(x => x.DiscipinaryEventType == DisciplinaryEventEnum.WarnEvent)
                                                                                .Where(x => x.UserID == userID)
                                                                                    .Where(x => x.DateToRemove <= DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)))
                                                                                        .CountAsync();
                    return warnCount;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task AddFilterAsync(string name, string pattern, FilterTypeEnum type)
        {
            try
            {
                using (FilterContext db = new FilterContext())
                {
                    await db.AddAsync(new FilterTable()
                    {
                        FilterName = name,
                        FilterPattern = pattern,
                        FilterType = type
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static List<ModeratedElement> GetModeratedWords()
        {
            try
            {
                List<ModeratedElement> result = new List<ModeratedElement>();
                using (FilterContext db = new FilterContext())
                {
                    foreach (FilterTable filter in db.filterTables.Where(x => x.FilterType == FilterTypeEnum.ContainFilter))
                    {
                        result.Add(new ModeratedElement()
                        {
                            Dialog = filter.FilterName,
                            Pattern = filter.FilterPattern
                        });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static List<ModeratedElement> GetModeratedRegex()
        {
            try
            {
                List<ModeratedElement> result = new List<ModeratedElement>();
                using (FilterContext db = new FilterContext())
                {
                    foreach (FilterTable filter in db.filterTables.Where(x => x.FilterType == FilterTypeEnum.RegexFilter))
                    {
                        result.Add(new ModeratedElement()
                        {
                            Dialog = filter.FilterName,
                            Pattern = filter.FilterPattern
                        });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<List<DisciplinaryEventEnum>> CheckPendingDisciplinaries(SocketGuildUser User)
        {
            try
            {
                //TODO this needs to catch multiple evnts
                var list = new List<DisciplinaryEventEnum>();
                using (var db = new UserContext())
                {
                    if (await db.UserStorageTable.AnyAsync(x => x.UserID == User.Id))
                    {
                        list.Add(await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == User.Id).Select(x => x.DiscipinaryEventType).FirstOrDefaultAsync());
                        if (list.Count > 0)
                        {
                            list.Add(await db.UserDisciplinaryPermanentStorageTable.Where(x => x.UserID == User.Id).Select(x => x.DiscipinaryEventType).FirstOrDefaultAsync());

                        }                    
                    }
                }

                return list;

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}

