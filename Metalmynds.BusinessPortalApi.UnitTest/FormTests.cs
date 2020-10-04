using Metalmynds.BusinessPortalApi.Client;
using Metalmynds.BusinessPortalApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metalmynds.BusinessPortalApi.UnitTest
{
    [TestClass]
    public class FormTests
    {

        [TestMethod]
        public void CreateNewSelectiveRuleTest()
        {
            var newRule = new SelectiveCallRule()
            {
                Id = "New Rule",
                Name = "New Rule",                
                Forward = ForwardTo.UsePhoneNumberorSipUri,
                PhoneNumberOrSipUrl = "123567890",
                AcceptCalls = AcceptCallsFrom.AllPhoneNumbers,
                Enabled = true,
                AcceptPrivateNumbers = false,
                AcceptUnknownNumbers = false,
                HolidaySchedule = "",
                TimeSchedule = ""
            };

            var fields = Forms.CreateSelectiveRule(newRule);



        }

    }
}