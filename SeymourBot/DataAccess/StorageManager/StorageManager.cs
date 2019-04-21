﻿using Discord.WebSocket;
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


                    return result.CommandName;
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

                    var existingDisciplinaryEvent = await db.UserDisciplinaryEventStorageTable.FirstOrDefaultAsync(x => x.UserID == newUser.UserID);

                    if (existingDisciplinaryEvent != null)
                    {
                        existingDisciplinaryEvent = newEvent;
                        await db.SaveChangesAsync();
                        return new KeyValuePair<ulong, bool>(newEvent.DisciplineEventID, true);
                    }
                    else
                    {
                        await db.AddAsync(newEvent);
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

        public static async Task RemoveDisciplinaryEventAsync(ulong userID)
        {
            try
            {
                using (var db = new UserContext())
                {
                    var existingEvent = await db.UserDisciplinaryEventStorageTable.FirstOrDefaultAsync(x => x.UserID == userID);

                    if (existingEvent != null)
                    {

                        await TimedEventManager.RemoveEvent(existingEvent.DisciplineEventID);
                        db.UserDisciplinaryEventStorageTable.Remove(existingEvent);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var existingPermaEvent = await db.UserDisciplinaryPermanentStorageTable.FirstOrDefaultAsync(x => x.UserID == userID);

                        if (existingPermaEvent != null)
                        {
                            await TimedEventManager.RemoveEvent(existingPermaEvent.DisciplineEventID);
                            db.UserDisciplinaryPermanentStorageTable.Remove(existingPermaEvent);
                            await db.SaveChangesAsync();
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

