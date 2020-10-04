using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Metalmynds.BusinessPortalApi.Model
{
    public class SelectiveCallRule
    {
        public String Id { get; set; }

        public String Name { get; set; }

        public ForwardTo Forward { get; set; }

        public String PhoneNumberOrSipUrl { get; set; }

        public String TimeSchedule { get; set; }

        public String HolidaySchedule { get; set; }

        public AcceptCallsFrom AcceptCalls { get; set; }

        public Boolean AcceptPrivateNumbers { get; set; }

        public Boolean AcceptUnknownNumbers { get; set; }
        
        public Boolean Enabled { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ForwardTo
    {
        UseDefaultForward = 1,
        UsePhoneNumberorSipUri = 2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AcceptCallsFrom
    {
        AllPhoneNumbers = 1,
        OnlyThesePhoneNumbers = 2
    }
}
