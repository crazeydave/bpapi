using HtmlAgilityPack;
using Metalmynds.BusinessPortalApi.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Metalmynds.BusinessPortalApi.Client
{
    public static class Pages
    {

        public static Dictionary<String, String> GetFormFields(String pageSource, String path = "//form") {

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode(path);

            var controls = document.DocumentNode.SelectNodes("//input");

            var fields = new Dictionary<String, String>();

            controls.ToList<HtmlNode>().ForEach((control) =>
            {
                var name = control.GetAttributeValue("name", null) ?? control.GetAttributeValue("id", null);

                switch (control.Attributes["type"].Value)
                {
                    case "checkbox":
                        if (control.GetAttributeValue("checked", null) != null)
                        {
                            fields.Add(name, "on");
                        } 
                        break;
                    default:
                        fields.Add(name, control.GetAttributeValue("value", String.Empty));
                        break;
                }                
            });

            return fields;

        }

        public static Dictionary<String, String> RemoveFields(String[] names, Dictionary<String, String> fields)
        {
            var list = from field in fields where !names.Contains<String>(field.Key) select field;

            return new Dictionary<string, string>(list);
        }

        public static Dictionary<String, Boolean> GetSelectiveCallListFields(String pageSource, String path = "//form")
        {

            var fields = GetFormFields(pageSource);

            var states = from field in fields
                         let name = field.Key 
                         let postion = Forms.GetSelectiveRuleIndex(name, fields)
                         let state = Forms.IsSelectiveRuleEnabled(name, fields)
                         select new KeyValuePair<String, Boolean>(name, state);      

            return new Dictionary<string, Boolean>(states);
        }


        public static Dictionary<String, String> GetUserTimeScheduleListFields(String pageSource, String path = "//form")
        {

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode(path);

            //var fields = document.DocumentNode.SelectNodes("//input[@type='checkbox' or @type='hidden']");

            var links = document.DocumentNode.SelectNodes("//td/a");

            var fields = (from link in links
                          let address = WebUtility.UrlDecode(link.Attributes["href"].Value)
                          let parameter = address.Split('&')
                         select parameter[1].Split("=")[1]).ToArray();
                         
            var values = fields.Select((name, index) => new KeyValuePair<String, String> ($"slot[{index}]", name));


            //var values = from field in fields
            //             let type = field.GetAttributeValue("type", null)
            //             let name = field.GetAttributeValue("name", null) ?? field.GetAttributeValue("id", null)
            //             let value = type == "checkbox" && field.GetAttributeValue("checked", String.Empty) == "checked" ? "checked" : field.GetAttributeValue("value", null)
            //             where !name.StartsWith("toDelete") && name != "SubmitButton"
            //             select new KeyValuePair<String, String>(name, value);

            return new Dictionary<String, String>(values);
        }


        public static Dictionary<String, String> GetTimeSlotListFields(String pageSource, String path = "//form")
        {

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode(path);

            var fields = document.DocumentNode.SelectNodes("//input[@type='checkbox' or @type='hidden']");

            var values = from field in fields
                         let type = field.GetAttributeValue("type", null)
                         let name = field.GetAttributeValue("name", null) ?? field.GetAttributeValue("id", null)
                         let value = type == "checkbox" && field.GetAttributeValue("checked", String.Empty) == "checked" ? "checked" : field.GetAttributeValue("value", null)
                         where !name.StartsWith("delete") && name != "selectAll"
                         select new KeyValuePair<String, String>(name, value);

            return new Dictionary<string, string>(values);
        }

        public static SelectiveCallRule ExtractSelectiveCallRule(String pageSource)
        {
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode("//form[@name='CallForwardingSelectiveForm']");

            var rule = new SelectiveCallRule()
            {
                Id = GetTextValue(form, "description"),
                Name = GetTextValue(form, "description"),
                Forward = GetRadioSelected(form, "useDefaultForwardToNumber") == "true" 
                    ? ForwardTo.UseDefaultForward : ForwardTo.UsePhoneNumberorSipUri,
                PhoneNumberOrSipUrl = GetTextValue(form, "forwardToNumber"),
                TimeSchedule = GetSelectedOptionValue(form, "newTimeSchedule").Replace("Personal", String.Empty),
                HolidaySchedule = HasSelectedOption(form, "newHolidaySchedule") 
                    ? GetSelectedOptionValue(form, "newHolidaySchedule").Replace("Group", String.Empty).Replace("Service Provider", String.Empty) : "None",
                AcceptCalls = GetRadioSelected(form, "usePhoneNumbers") == "All Phone Numbers" 
                    ? AcceptCallsFrom.AllPhoneNumbers : AcceptCallsFrom.OnlyThesePhoneNumbers
            };

            return rule;
        }

        public static UserTimeSchedule ExtractUserTimeSchedule(String pageSource)
        {
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(pageSource);

            var form = document.DocumentNode.SelectSingleNode("//form[@name='UserTimeScheduleForm']");

            var name = GetTextValue(form, "name").Replace("Personal", String.Empty);

            var user = new UserTimeSchedule()
            {
                Id = name,
                Name = name,
            };

            var startDays = form.SelectNodes("//select[@name[starts-with(.,'startDay')]]");

            var activeStartdays = from day in startDays
                                  where day.SelectSingleNode("./option[@selected = 'selected']") != null
                                  select day;

            var shifts = from activeDay in activeStartdays
                         let index = activeDay.GetAttributeValue("name", null).Replace("startDay[", String.Empty).Replace("]", String.Empty)
                         let startDay = (DayOfWeek)int.Parse(activeDay.SelectSingleNode("./option[@selected = 'selected']").GetAttributeValue("value", null))
                         let startTime = DateTime.Parse(form.SelectSingleNode($"//input[@name = 'startTime[{index}]']").GetAttributeValue("value", null), null, DateTimeStyles.NoCurrentDateDefault)
                         let endDay = (DayOfWeek)int.Parse(form.SelectSingleNode($"//select[@name='endDay[{index}]']/option[@selected = 'selected']").GetAttributeValue("value", null))
                         let endTime = DateTime.Parse(form.SelectSingleNode($"//input[@name = 'endTime[{index}]']").GetAttributeValue("value", null), null, DateTimeStyles.NoCurrentDateDefault)
                            select new UserTimeScheduleShift
                            {
                                StartDay = startDay,
                                StartTime = startTime,
                                EndDay = endDay,
                                EndTime = endTime
                            };

            user.Shifts = new List<UserTimeScheduleShift>(shifts);

            return user;
        }

        public static String GetTextValue(HtmlNode form, String name)
        {
            var input = form.SelectSingleNode(String.Format(".//input[@name='{0}']", name));

            return input.Attributes["value"].Value;
        }

        public static String GetRadioSelected(HtmlNode form, String name)
        {

            var inputs = form.SelectNodes(String.Format(".//input[@type='radio' and @name='{0}']", name));

            var selected = (from input in inputs
                             where input.GetAttributeValue("checked", "") == "checked"
                             select Sanitise(input.ParentNode.InnerText)).First();

            System.Diagnostics.Debug.Write(selected);

            return selected;
        }

        public static String GetSelectedOptionText(HtmlNode form, String name)
        {
            var input = form.SelectSingleNode(String.Format(".//select[@name='{0}']", name));

            var selected = input.SelectSingleNode(".//option[@selected='selected']");

            return Sanitise(selected.InnerText);
        }

        public static void SetSelectedOptionValue(HtmlNode form, String name, String value)
        {
            var input = form.SelectSingleNode(String.Format(".//select[@name='{0}']", name));

            var currentSelection = input.SelectSingleNode(".//option[@selected='selected']");

            currentSelection.Attributes.Remove("selected");

            var nextSelection = input.SelectSingleNode(String.Format(".//option[@value='{0}']", value));

            nextSelection.Attributes.Add("selected", "selected");

        }

        public static Boolean HasSelectedOption(HtmlNode form, String name)
        {
            var input = form.SelectSingleNode(String.Format(".//select[@name='{0}']", name));

            var selected = input.SelectSingleNode(".//option[@selected='selected']");

            return selected != null;
        }

        public static String GetSelectedOptionValue(HtmlNode form, String name)
        {
            var input = form.SelectSingleNode(String.Format(".//select[@name='{0}']", name));

            var selected = input.SelectSingleNode(".//option[@selected='selected']");

            var value = selected == null ? String.Empty : selected.Attributes["value"].Value;

            return value;
        }

        public static void SetTextValue(HtmlNode form, String name, String value)
        {
            var input = form.SelectSingleNode(String.Format(".//input[@name='{0}']", name));

            input.Attributes["value"].Value = value;
        }

        public static void SetRadioChecked(HtmlNode form, String name, String value)
        {

            var inputs = form.SelectNodes(String.Format(".//input[@type='radio' and @name='{0}']", name));

            var currentSelected = (from input in inputs
                                   where Sanitise(input.ParentNode.InnerText) == value
                                   select input).First();

            currentSelected.Attributes.Remove("checked");

            var nextSelected = (from input in inputs
                                where Sanitise(input.ParentNode.InnerText) == value
                                select input).First();

            nextSelected.Attributes.Add("checked", "checked");

        }

        public static Boolean GetCheckBox(HtmlNode form, String name)
        {
            var input = form.SelectSingleNode(String.Format(".//input[@type='checkbox' and @name='{0}']", name));

            return input.Attributes["value"].Value == "on";

        }

        public static void SetCheckBox(HtmlNode form, String name, Boolean value)
        {
            var input = form.SelectSingleNode(String.Format(".//input[@type='checkbox' and @name='{0}']", name));

            input.Attributes["value"].Value = (value ? "on" : "off");

        }

        private static String Sanitise(String text)
        {
            return text.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();
        }

    }
}
