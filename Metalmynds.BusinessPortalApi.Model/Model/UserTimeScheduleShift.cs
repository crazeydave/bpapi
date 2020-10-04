using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Metalmynds.BusinessPortalApi.Model
{
    public class UserTimeScheduleShift
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek StartDay { get; set; }
        
        public DateTime StartTime { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek EndDay { get; set; }

        public DateTime EndTime { get; set; }
    }
}
