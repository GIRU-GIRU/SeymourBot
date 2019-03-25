using SeymourBot.DataAccess.StorageManager;
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

        public static event EventHandler<ActiveTimedEvent> OnEventElapsed;

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
            ActiveTimedEvent activeEvent;
            foreach (var timedEvent in dbTimedEvents)
            {
                activeEvent = new ActiveTimedEvent();
                activeEvent.DisciplinaryEvent = timedEvent.DiscipinaryEventType;
                activeEvent.TimeToTrigger = currentTime.Subtract(timedEvent.DateToRemove).Minutes;
            }
        }

        public async static void CreateEvent(UserDisciplinaryEventStorage newEvent, UserStorage newUser)
        {
            var activeEvent = new ActiveTimedEvent();
            activeEvent.DisciplinaryEvent = newEvent.DiscipinaryEventType;
            activeEvent.TimeToTrigger = newEvent.DateToRemove.Subtract(newEvent.DateInserted).Minutes;
            activeEvents.Add(activeEvent);
            await StorageManager.StoreTimedEvent(newEvent, newUser);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (ActiveTimedEvent activeEvent in activeEvents)
            {
                if (--activeEvent.TimeToTrigger <= 0)
                {
                    OnEventElapsed.Invoke(null, activeEvent);
                }
            }
        }
    }
}
