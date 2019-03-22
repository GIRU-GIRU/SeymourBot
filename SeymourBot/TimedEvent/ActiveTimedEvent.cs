using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.TimedEvent
{
    class ActiveTimedEvent
    {
        //in minutes
        private int timeToTrigger;

        public int TimeToTrigger { get => timeToTrigger; set => timeToTrigger = value; }
    }
}
