using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class BusinessPortalConfiguration
    {
        public BusinessPortalConfiguration()
        {

        }

        public String Organisation { get; set; }

        public String User { get; set; }

        public String Domain { get; set; }

        public String Password { get; set; }

        public int TimeZoneOffset { get; set; } = 60;

        public Boolean ShouldSetDomain { get; set; } = true;

        public String BaseUrl { get; set; }

        public int TimeoutMinutes { get; set; } = 3;

        public String RegKey
        {
            get { return $"UsersOrgUnit=Users,UsersOrgUnit=Customers,UsersOrgUnit=BTCloudVoice,Organization={Organisation},GroupUsersOrgUnit=Users,User={User},Registration=BTCloudVoice Registration"; }
        }
    }
}