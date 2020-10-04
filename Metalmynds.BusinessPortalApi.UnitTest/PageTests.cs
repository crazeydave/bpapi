using Metalmynds.BusinessPortalApi.Client;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalmynds.BusinessPortalApi.UnitTest
{
    [TestClass]
    public class PageTests
    {

        [TestMethod]
        public void GetUserTimeScheduleFields()
        {

        }

        [TestMethod]
        public void GetSelectiveCallList()
        {
            string rulename = "AB1 Monday Afternoon - Office A";

            var pageSource = File.ReadAllText("./TestData/SelectiveCallRulesListGet.html");

            var fields = Pages.GetFormFields(pageSource);

            var index = Forms.GetSelectiveRuleIndex(rulename, fields);
            
            Assert.IsTrue(index == "1", $"Index of {rulename} in Selective Call List failed!");

        }

        [TestMethod]
        public void ExtractUserTimeSchedule()
        {

            var pageSource = File.ReadAllText("./TestData/UserTimeScheduleForm.html");

            var userSchedule = Pages.ExtractUserTimeSchedule(pageSource);

            

        }

        [TestMethod]
        public void MineSelectiveRuleFormFields()
        {
            var document = new HtmlDocument();

            var pageSource = File.ReadAllText("./TestData/SelectiveCallRuleForm.html");

            //Pages.GetSelectiveCallListFields(pageSource);

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode("//form[@name='CallForwardingSelectiveForm']");

            // Parse Form to Get Values instead

            var description = Pages.GetTextValue(form, "description");

            Assert.IsTrue(description == "ZZ Test Record ot Used", "Description value not expected!");

            var isChecked = Pages.GetRadioSelected(form, "useDefaultForwardToNumber");

            Assert.IsTrue(isChecked == "Use Default Forward To Number/SIP URI", "Checked radio is not expected!");

            var isSelectedValue = Pages.GetSelectedOptionValue(form, "newTimeSchedule");

            Assert.IsTrue(isSelectedValue == "Every Day All DayPersonal", "Selected Option is not expected!");

            var isSelectedText = Pages.GetSelectedOptionText(form, "newTimeSchedule");

            Assert.IsTrue(isSelectedText.StartsWith("Every Day All Day"), "Selected Option is not expected!");

        }
    }
}