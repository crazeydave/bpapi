using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Model
{
    public class UserTimeSchedule
    {
        public String Id { get; set; }

        public String Name { get; set; }

        public List<UserTimeScheduleShift> Shifts { get; set; }

        public UserTimeSchedule()
        {

        }

        public UserTimeSchedule(String id)
        {
            Id = id;
            Name = id;
        }
    }
}
