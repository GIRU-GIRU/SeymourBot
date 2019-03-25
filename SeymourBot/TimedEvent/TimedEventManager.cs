using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Storage;
using System;
using System.Collections.Generic;
using System.Text;
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

        private async static void LoadFromDB()
        {
            activeEvents = new List<ActiveTimedEvent>();
            DateTime currentTime = DateTime.Now;
            var dbTimedEvents = await StorageManager.GetTimedEvents();
            foreach (var timedEvent in dbTimedEvents)
            {
                activeEvents.Add(BuildActiveTimedEvent(timedEvent));
            }
        }

        public async static void CreateEvent(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            activeEvents.Add(BuildActiveTimedEvent(newEvent));
            await StorageManager.StoreTimedEvent(newEvent, newUser);
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

        private static async void HandleEventElapsed(ActiveTimedEvent activeEvent)
        {
            switch (activeEvent.DisciplinaryEvent)
            {
                case Storage.User.DisciplineEventEnum.MuteEvent:
                    await DiscordContext.RemoveRole(activeEvent.UserId, ConfigManager.GetUlongUserSetting(PropertyItem.Role_Muted));
                    break;
                default:
                    break;
            }
        }
    }
}
