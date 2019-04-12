using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

        public async static Task CreateEvent(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            await HandleEventCreated(newEvent, newUser);
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

        private static async Task HandleEventCreated(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            var newActiveEvent = BuildActiveTimedEvent(newEvent);
            activeEvents.Add(newActiveEvent);
            var id = await StorageManager.StoreTimedEventAsync(newEvent, newUser);
            newActiveEvent.DisciplinaryEventId = id;
            try
            {
                switch (newEvent.DiscipinaryEventType)
                {
                    case Storage.User.DisciplinaryEventEnum.MuteEvent:
                        await DiscordContext.AddRole(DiscordContext.GrabRole(MordhauRoleEnum.Muted), newEvent.UserID);
                        break;
                    case Storage.User.DisciplinaryEventEnum.WarnEvent:
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
