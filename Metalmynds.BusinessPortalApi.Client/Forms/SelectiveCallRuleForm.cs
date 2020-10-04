using Metalmynds.BusinessPortalApi.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metalmynds.BusinessPortalApi.Client.Forms
{
    public class SelectiveCallRuleForm
    {

        private SelectiveCallRuleForm(String pageSource)
        {

        }

        public static SelectiveCallRuleForm Create() { throw new NotImplementedException(); }

        public static SelectiveCallRuleForm Get() { throw new NotImplementedException(); }

        public static SelectiveCallRuleForm Set() { throw new NotImplementedException(); }

        public Dictionary<String,String> Fields
        {
            get;
        }

        public SelectiveCallRule Rule { get; } 

        public Boolean Updated { get; }
    }
}
