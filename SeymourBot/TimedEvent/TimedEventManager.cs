using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Microsoft.EntityFrameworkCore;
using Toolbox.Exceptions;

namespace SeymourBot.TimedEvent
{
    static class TimedEventManager
    {
        private static List<ActiveTimedEvent> ActiveEvents;
        private static Timer Timer;

        static TimedEventManager()
        {
            Timer = new Timer();
            Timer.AutoReset = true;
            Timer.Interval = 60000; //tick every minute
            Timer.Elapsed += Timer_Elapsed;
            _ = LoadFromDB();
            Timer.Start();
        }

        private async static Task LoadFromDB()
        {
            ActiveEvents = new List<ActiveTimedEvent>();
            var dbTimedEvents = await StorageManager.GetTimedEvents();
            foreach (var timedEvent in dbTimedEvents)
            {
                ActiveEvents.Add(BuildActiveTimedEvent(timedEvent));
            }
        }

        public async static Task RemoveEvent(ulong eventID)
        {
            try
            {
                var eventToRemove = ActiveEvents.FirstOrDefault(x => x.DisciplinaryEventId == eventID);
                ActiveEvents.Remove(eventToRemove);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }

        }

        public async static Task<bool> CreateEvent(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            return await HandleEventCreated(newEvent, newUser);
        }

        public async static Task CreateEvent(DisciplinaryEventEnum eventType, ulong moderatorId, string reason, ulong userId, string userName, DateTime end)
        {
            UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
            {
                DateInserted = DateTime.UtcNow,
                DateToRemove = end,
                DiscipinaryEventType = eventType,
                ModeratorID = moderatorId,
                Reason = reason,
                UserID = userId
            };
            UserStorage newUser = new UserStorage()
            {
                UserID = userId,
                UserName = userName
            };
            await HandleEventCreated(newEvent, newUser);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ActiveTimedEvent[] events = ActiveEvents.ToArray(); //fix for a vulnerability (event being added while checking elapsed events cause crash)
            foreach (ActiveTimedEvent activeEvent in events)
            {
                if (--activeEvent.TimeToTrigger <= 0)
                {
                    _ = HandleEventElapsed(activeEvent);
                }
            }
        }

        private static ActiveTimedEvent BuildActiveTimedEvent(UserDisciplinaryEventStorage eventStorage)
        {
            var activeEvent = new ActiveTimedEvent();
            activeEvent.DisciplinaryEvent = eventStorage.DiscipinaryEventType;
            activeEvent.TimeToTrigger = eventStorage.DateToRemove.Subtract(eventStorage.DateInserted).TotalMinutes;
            activeEvent.UserId = eventStorage.UserID;
            return activeEvent;
        }

        private static async Task<bool> HandleEventCreated(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            try
            {
                var newActiveEvent = BuildActiveTimedEvent(newEvent);
                ActiveEvents.Add(newActiveEvent);
                var result = await StorageManager.StoreTimedEventAsync(newEvent, newUser);
                newActiveEvent.DisciplinaryEventId = result.Key;

                return result.Value;
            }
            catch (Exception ex)
            {
                await ExceptionManager.LogExceptionAsync(ex);
                throw;
            }
        }

        private static async Task HandleEventElapsed(ActiveTimedEvent activeEvent)
        {
            try
            {
                switch (activeEvent.DisciplinaryEvent)
                {
                    case Storage.User.DisciplinaryEventEnum.MuteEvent:
                        ActiveEvents.Remove(activeEvent);
                        //todo
                        await DiscordContext.RemoveRoleAsync(activeEvent.UserId, ConfigManager.GetUlongProperty(PropertyItem.Role_Muted));
                        await StorageManager.ArchiveTimedEventAsync(activeEvent.DisciplinaryEventId);
                        break;
                    case Storage.User.DisciplinaryEventEnum.WarnEvent:
                        ActiveEvents.Remove(activeEvent);
                        await StorageManager.ArchiveTimedEventAsync(activeEvent.DisciplinaryEventId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await ExceptionManager.LogExceptionAsync(ex);
            }
        }
    }
}
