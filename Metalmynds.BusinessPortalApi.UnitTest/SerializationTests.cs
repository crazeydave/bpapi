using Metalmynds.BusinessPortalApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Metalmynds.BusinessPortalApi.UnitTest
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void DeserializeSelectiveRule()
        {
            var source = File.ReadAllText("./TestData/SelectiveCallRule.json");

            var options = new JsonSerializerOptions();

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var rule = JsonSerializer.Deserialize<SelectiveCallRule>(source, options);

            Assert.IsTrue(rule.Id == "ZZZ Dave Test 1");
            Assert.IsTrue(rule.Name == "ZZZ Dave Test 1");
            Assert.IsTrue(rule.Forward == ForwardTo.UsePhoneNumberorSipUri);
            Assert.IsTrue(rule.TimeSchedule == "A Friday 10:00 - 14:00");
            Assert.IsTrue(rule.HolidaySchedule == "None");
            Assert.IsTrue(rule.AcceptCalls == AcceptCallsFrom.AllPhoneNumbers);
            Assert.IsTrue(rule.AcceptPrivateNumbers == false);
            Assert.IsTrue(rule.AcceptUnknownNumbers == false);


        }

    }
}
