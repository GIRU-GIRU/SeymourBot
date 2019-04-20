using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;

namespace SeymourBot.TimedEvent
{
    static class TimedEventManager
    {
        private static List<ActiveTimedEvent> activeEvents;
        private static Timer timer;

        static TimedEventManager()
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 60000; //tick every minute
            timer.Elapsed += Timer_Elapsed;
            _ = LoadFromDB();
            timer.Start();
        }

        private async static Task LoadFromDB()
        {
            activeEvents = new List<ActiveTimedEvent>();
            var dbTimedEvents = await StorageManager.GetTimedEvents();
            foreach (var timedEvent in dbTimedEvents)
            {
                activeEvents.Add(BuildActiveTimedEvent(timedEvent));
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
                DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
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
            ActiveTimedEvent[] events = activeEvents.ToArray(); //fix for a vulnerability (event being added while checking elapsed events cause crash)
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
            activeEvent.TimeToTrigger = eventStorage.DateToRemove.Subtract(eventStorage.DateInserted).Minutes;
            activeEvent.UserId = eventStorage.UserID;
            return activeEvent;
        }

        private static async Task<bool> HandleEventCreated(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            try
            {
                var newActiveEvent = BuildActiveTimedEvent(newEvent);
                activeEvents.Add(newActiveEvent);
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
                        activeEvents.Remove(activeEvent);
                        //todo
                        await DiscordContext.RemoveRoleAsync(activeEvent.UserId, ConfigManager.GetUlongProperty(PropertyItem.Role_Muted));
                        await StorageManager.ArchiveTimedEventAsync(activeEvent.DisciplinaryEventId);
                        break;
                    case Storage.User.DisciplinaryEventEnum.WarnEvent:
                        activeEvents.Remove(activeEvent);
                        await StorageManager.ArchiveTimedEventAsync(activeEvent.DisciplinaryEventId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.LogExceptionAsync(ex);
            }
        }
    }
}
