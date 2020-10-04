using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class ParsePageFailedException : Exception
    {
        public ParsePageFailedException(String expression, String source)
            : base($"Failed Parsing Html!\nExpression: {expression}\nSource:\n{source}")
        {

        }

    }
}
