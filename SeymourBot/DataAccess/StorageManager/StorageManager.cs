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
using SeymourBot.Storage.User.Tables;
using SeymourBot.TimedEvent;
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
        public static async Task<bool> DeleteInfoCommandAsync(string name)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {
                    var cmd = await db.InfoCommandTable.Where(x => x.CommandName.ToLower() == name).FirstOrDefaultAsync();

                    if (cmd != null)
                    {
                        db.InfoCommandTable.Remove(cmd);
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }

        }

        public static async Task<bool> StoreInfoCommandAsync(Command command)
        {
            try
            {
                using (InfoCommandContext db = new InfoCommandContext())
                {
                    var cmd = await db.InfoCommandTable.FirstOrDefaultAsync(x => x.CommandName.ToLower() == command.CommandName.ToLower());

                    if (cmd == null)
                    {
                        await db.InfoCommandTable.AddAsync(new InfoCommandTable
                        {
                            CommandName = command.CommandName,
                            CommandContent = command.CommandContent
                        });

                        await db.SaveChangesAsync();
                        return false;
                    }
                    else
                    {
                        cmd = new InfoCommandTable
                        {
                            CommandName = command.CommandName,
                            CommandContent = command.CommandContent
                        };

                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
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

                    if (result != null)
                    {
                        return result.CommandContent;
                    }
                    else
                    {
                        return string.Empty;
                    }            
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<KeyValuePair<ulong, bool>> StoreTimedEventAsync(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
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

                    var existingDisciplinaryEvent = await db.UserDisciplinaryEventStorageTable.FirstOrDefaultAsync(x => x.UserID == newUser.UserID && x.DiscipinaryEventType == newEvent.DiscipinaryEventType);

                    if (existingDisciplinaryEvent != null && newEvent.DiscipinaryEventType != DisciplinaryEventEnum.WarnEvent)
                    {
                        existingDisciplinaryEvent = newEvent;
                        await db.SaveChangesAsync();
                        return new KeyValuePair<ulong, bool>(newEvent.DisciplineEventID, true);
                    }
                    else
                    {
                        await db.UserDisciplinaryEventStorageTable.AddAsync(newEvent);
                        await db.SaveChangesAsync();
                        return new KeyValuePair<ulong, bool>(newEvent.DisciplineEventID, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<Dictionary<string, string>> GetDisciplinariesAsync(SocketGuildUser user)
        {
            try
            {
                using (var db = new UserContext())
                {
                    var currentEvents = await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == user.Id).ToListAsync();
                    var archivedEvents = await db.UserDisciplinaryEventArchiveTable.Where(x => x.UserID == user.Id).ToListAsync();
                    var permaEvents = await db.UserDisciplinaryPermanentStorageTable.Where(x => x.UserID == user.Id).ToListAsync();

                    var dict = new Dictionary<string, string>();
                    int index = 0;
                    string type = string.Empty;
                    string reason = string.Empty;
                    if (currentEvents != null)
                    {
                        foreach (var item in currentEvents)
                        {
                            type = item.DiscipinaryEventType.ToString().Replace("Event", String.Empty);
                            if (!String.IsNullOrEmpty(item.Reason)) reason = $", {item.Reason}";

                            dict.Add($"{index}: {item.DateInserted.ToShortDateString()}", $"CURRENT - {type}{reason}");
                            index++;
                        }
                    }
                    if (archivedEvents != null)
                    {
                        foreach (var item in archivedEvents)
                        {
                            type = item.DisciplineType.ToString().Replace("Event", String.Empty);
                            if (!String.IsNullOrEmpty(item.Reason)) reason = $", {item.Reason}";

                            dict.Add($"{index}: {item.DateInserted.ToShortDateString()}", $"ARCHIVED - {type}{reason}");
                            index++;
                        }
                    }
                    if (permaEvents != null)
                    {
                        foreach (var item in permaEvents)
                        {
                            type = item.DiscipinaryEventType.ToString().Replace("Event", String.Empty);
                            if (!String.IsNullOrEmpty(item.Reason)) reason = $", {item.Reason}";

                            dict.Add($"{index}: {item.DateInserted.ToShortDateString()}", $"PERMA - {type}{reason}");
                            index++;
                        }
                    }

                    return dict;
                }
            }
            catch (Exception ex)
            {
                //todo
                throw ex;
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
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
            }
        }

        public static async Task<bool> RemoveActiveDisciplinaries(ulong userID)
        {
            try
            {
                using (var db = new UserContext())
                {
                    var activeDisciplinaries = await db.UserDisciplinaryEventStorageTable.AnyAsync(x => x.UserID == userID);

                    if (activeDisciplinaries)
                    {
                        var itemsToRemove = await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == userID).ToListAsync();

                        db.RemoveRange(itemsToRemove);
                        await db.SaveChangesAsync();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;//todo
            }
        }

        public static async Task RemoveDisciplinaryEventAsync(ulong userID, DisciplinaryEventEnum type)
        {
            try
            {
                using (var db = new UserContext())
                {
                    //check event table first
                    UserDisciplinaryEventStorage[] existingEvents;

                    if (type == DisciplinaryEventEnum.BanEvent)
                    {
                        existingEvents = await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == userID).ToArrayAsync();
                    }
                    else
                    {
                        existingEvents = await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == userID
                                                                                            && x.DiscipinaryEventType == type).ToArrayAsync();
                    }

                    if (existingEvents.Count() > 0)
                    {
                        foreach (var item in existingEvents)
                        {
                            await TimedEventManager.RemoveEvent(item.DisciplineEventID);
                        }

                    }
                    else //check permanent event table if can't find
                    {
                        UserDisciplinaryPermanentStorage[] existingPermaEvents;

                        if (type == DisciplinaryEventEnum.BanEvent)
                        {
                            existingPermaEvents = await db.UserDisciplinaryPermanentStorageTable.Where(x => x.UserID == userID).ToArrayAsync();
                        }
                        else
                        {
                            existingPermaEvents = await db.UserDisciplinaryPermanentStorageTable.Where(x => x.UserID == userID
                                                                                                && x.DiscipinaryEventType == type).ToArrayAsync();
                        }

                        if (existingPermaEvents.Count() > 0)
                        {
                            foreach (var item in existingPermaEvents)
                            {
                                await TimedEventManager.RemoveEvent(item.DisciplineEventID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
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

        public static async Task<bool> StoreBlacklistReturnIfExisting(BlacklistUserStorage obj, UserStorage user)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    if (!await db.UserStorageTable.AnyAsync(x => x.UserID == user.UserID))
                    {
                        await db.AddAsync(user);
                    }

                    var existingBlacklist = await db.BlackListedTable.FirstOrDefaultAsync(x => x.UserID == user.UserID);

                    if (existingBlacklist != null)
                    {
                        existingBlacklist = obj;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        await db.AddAsync(obj);
                        await db.SaveChangesAsync();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<bool> RemoveBlacklist(ulong userID)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var blacklistedUser = await db.BlackListedTable.FirstOrDefaultAsync(x => x.UserID == userID);

                    if (blacklistedUser != null)
                    {
                        db.BlackListedTable.Remove(blacklistedUser);
                        await db.SaveChangesAsync();
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.StorageException, ex);
                throw;
            }
        }

        public static async Task<bool> UserCheckAndUpdateBlacklist(SocketGuildUser user)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var existingBlacklist = await db.BlackListedTable.FirstOrDefaultAsync(x => x.UserID == user.Id);

                    if (existingBlacklist == null)
                    {
                        return false;
                    }

                    if (existingBlacklist.DateToRemove <= DateTime.UtcNow)
                    {
                        db.Remove(existingBlacklist);
                        await db.SaveChangesAsync();
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        public static async Task<bool> StoreDisciplinaryPermanentEventAsync(UserDisciplinaryPermanentStorage obj, UserStorage user)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    if (!await db.UserStorageTable.AnyAsync(x => x.UserID == user.UserID))
                    {
                        await db.AddAsync(user);
                    }


                    var existingEvent = await db.UserDisciplinaryPermanentStorageTable.FirstOrDefaultAsync(x => x.UserID == user.UserID);

                    if (existingEvent != null)
                    {
                        existingEvent = obj;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        await db.AddAsync(obj);
                        await db.SaveChangesAsync();
                        return false;
                    }
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
                    var warnDuration = ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration);
                    int warnCount = await db.UserDisciplinaryEventStorageTable.Where(x => x.DiscipinaryEventType == DisciplinaryEventEnum.WarnEvent)
                                                                                .Where(x => x.UserID == userID)
                                                                                    .Where(x => x.DateToRemove <= DateTime.UtcNow.AddDays(warnDuration))
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


        public static async Task RemoveFilterAsync(string name, FilterTypeEnum type)
        {
            try
            {
                using (FilterContext db = new FilterContext())
                {
                    var itemToRemove = await db.filterTables.FirstOrDefaultAsync(x => x.FilterPattern.ToLower() == name.ToLower()
                                                                                                                & x.FilterType == type);
                    if (itemToRemove != null)
                    {
                        db.filterTables.Remove(itemToRemove);
                        await db.SaveChangesAsync();
                    }
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
                var list = new List<DisciplinaryEventEnum>();
                using (var db = new UserContext())
                {
                    if (await db.UserStorageTable.AnyAsync(x => x.UserID == User.Id))
                    {
                        list.AddRange(await db.UserDisciplinaryEventStorageTable.Where(x => x.UserID == User.Id).Select(x => x.DiscipinaryEventType).ToListAsync());
                        if (list.Count > 0)
                        {
                            list.AddRange(await db.UserDisciplinaryPermanentStorageTable.Where(x => x.UserID == User.Id).Select(x => x.DiscipinaryEventType).ToListAsync());
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

