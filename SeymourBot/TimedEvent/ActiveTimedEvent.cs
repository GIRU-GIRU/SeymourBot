using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.TimedEvent
{
    class ActiveTimedEvent
    {
        //in minutes
        private double timeToTrigger;
        private DisciplinaryEventEnum disciplinaryEvent;
        private ulong userId;
        private ulong disciplinaryEventId;
        private string reason;

        public double TimeToTrigger { get => timeToTrigger; set => timeToTrigger = value; }
        public DisciplinaryEventEnum DisciplinaryEvent { get => disciplinaryEvent; set => disciplinaryEvent = value; }
        public ulong UserId { get => userId; set => userId = value; }
        public ulong DisciplinaryEventId { get => disciplinaryEventId; set => disciplinaryEventId = value; }
        public string Reason { get => reason; set => reason = value; }
    }
}
