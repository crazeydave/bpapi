using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class BusinessPortalLoginFailedException : Exception
    {
        public BusinessPortalLoginFailedException(String domain, String username, String password)
            : base (String.Format("Login to Business Portal Failed! Domain [{0}] Username [{1}] Password [{2}]", domain, username, password))
        {
        }

    }
}
