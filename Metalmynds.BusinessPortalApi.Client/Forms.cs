using Metalmynds.BusinessPortalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class Forms
    {
        public static Regex extractSelectiveRuleIndexExpression = new Regex("description\\[(?<index>\\d*)\\]", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static Dictionary<String, String> CreateSelectiveRule(SelectiveCallRule rule)
        {

            var fields = new Dictionary<String, String>
            {
                { "oldDescription", "" },
                { "operation", "add" },
                { "enabled", "false" }, // Ignoring Enabled as its allways true (even when enabled is false).
                //{ "enabled", enabled ? "true" : "false" },
                { "SubmitButton", "Ok" },
                { "description", String.IsNullOrWhiteSpace(rule.Id) ? rule.Name : rule.Id },
                { "useDefaultForwardToNumber", rule.Forward == ForwardTo.UseDefaultForward ? "true" : "false" },
                { "forwardToNumber", rule.PhoneNumberOrSipUrl },
                { "newTimeSchedule", rule.TimeSchedule + "Personal" },
                { "newHolidaySchedule", rule.HolidaySchedule + "Group" }
            };

            return fields;
        }

        public static Dictionary<String, String> UpdateSelectiveRule(SelectiveCallRule rule)
        {

            var fields = new Dictionary<String, String>
            {
                { "oldDescription", rule.Name != rule.Id ? rule.Id : rule.Name },
                { "operation", "modify" },
                { "enabled", "false" }, // Ignoring Enabled as its allways true (even when enabled is false).
                //{ "enabled", enabled ? "true" : "false" },
                { "SubmitButton", "Ok" },
                { "description", rule.Name != rule.Id ? rule.Name : rule.Id },
                { "useDefaultForwardToNumber", rule.Forward == ForwardTo.UseDefaultForward ? "true" : "false" },
                { "forwardToNumber", rule.PhoneNumberOrSipUrl },
                { "newTimeSchedule", rule.TimeSchedule + "Personal" },
                { "newHolidaySchedule", rule.HolidaySchedule == "None" ?  "NoneGroup" : rule.HolidaySchedule + "Service Provider" }
            };

            return fields;
        }

        public static String GetSelectiveRuleIndex(String name, Dictionary<String, String> fields)
        {
            var index = (from field in fields
                         where field.Value.EndsWith(name)
                         let position = extractSelectiveRuleIndexExpression.Match(field.Key).Groups[1].Value
                         select position).First();

            return index;
        }
       
        public static Boolean IsSelectiveRuleEnabled(String rule, Dictionary<String, String> fields)
        {
            var index = GetSelectiveRuleIndex(rule, fields);

            String value;

            if (fields.TryGetValue(String.Format("status[{0}]", index), out value))
            {
                return value == "on";
            }
            else
            {
                return false;
            }

        }

        public static void EnableSelectiveRule(String rule, Dictionary<String, String> fields)
        {
            var index = GetSelectiveRuleIndex(rule, fields);

            var key = String.Format("status[{0}]", index);

            if (!fields.Keys.Contains(key))
            {

                fields.Add(key, "on");
            };

        }

        public static void DisableSelectiveRule(String description, Dictionary<String, String> fields)
        {
            var index = GetSelectiveRuleIndex(description, fields);

            var key = String.Format("status[{0}]", index);

            if (fields.Keys.Contains(key))
            {
                fields.Remove(key);
            }
        }

        public static Dictionary<String, String> RemoveFields(String[] names, Dictionary<String, String> fields)
        {
            var list = from field in fields 
                       from name in names where !field.Key.StartsWith(name)
                       select field;

            return new Dictionary<string, string>(list);
        }


        public static void DeleteSelectiveRule(String description, Dictionary<String, String> fields)
        {
            throw new NotImplementedException();
        }
    }

}
