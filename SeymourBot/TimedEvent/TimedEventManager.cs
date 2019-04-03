using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Storage;
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
            LoadFromDB();
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
            var newActiveEvent = BuildActiveTimedEvent(newEvent);
            activeEvents.Add(newActiveEvent);
            var id = await StorageManager.StoreTimedEventAsync(newEvent, newUser);
            newActiveEvent.DisciplinaryEventId = id;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (ActiveTimedEvent activeEvent in activeEvents)
            {
                if (--activeEvent.TimeToTrigger <= 0)
                {
                    HandleEventElapsed(activeEvent);
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
