using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.TimedEvent
{
    static class TimedEventManager
    {
        private static List<ActiveTimedEvent> activeEvents;

        static TimedEventManager()
        {
            activeEvents = new List<ActiveTimedEvent>();
        }
    }
}
