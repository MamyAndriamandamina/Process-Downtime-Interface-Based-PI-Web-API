using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessDowntimeWebApplication.Models
{
    public class Event
    {
            public string WebId { get; set; }
            public DateTime StartTime { get; set; }
            public  DateTime EndTime { get; set; }
            public string AcknowledgedBy { get; set; }
            public string Duration { get; set; }
            public string AcknowledgedDate { get; set; }
            public string PrimaryReferencedElement { get; set; }
            public float downtimepercent { get; set; }
            public float downtimepercentstart { get; set; }
            public string Description_One { get; set; }
            public string Description_Two { get; set; }
            public string Description_Three { get; set; }
    }
}
