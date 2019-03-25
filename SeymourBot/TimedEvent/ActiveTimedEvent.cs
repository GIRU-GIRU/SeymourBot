using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.TimedEvent
{
    class ActiveTimedEvent
    {
        //in minutes
        private int timeToTrigger;
        private DisciplineEventEnum disciplinaryEvent;
        private ulong userId;

        public int TimeToTrigger { get => timeToTrigger; set => timeToTrigger = value; }
        public DisciplineEventEnum DisciplinaryEvent { get => disciplinaryEvent; set => disciplinaryEvent = value; }
        public ulong UserId { get => userId; set => userId = value; }
    }
}
