using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeTests
{
    public enum HydraStatus
    {
        Unknown = 0,
        Ready = 1,
        WaitingForTrigger = 2,
        Triggered = 4,
        Scanning = 8,
        Delay = 16,
        WaitingForAlarm = 32,
        Alarm = 64
    }

}
