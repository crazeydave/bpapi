using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class PortalClientErrorException : Exception
    {
        public PortalClientErrorException(String action,String area, String name)
            : base($"{action} {area} failed! Name: {name}")
        {

        }

    }
}
