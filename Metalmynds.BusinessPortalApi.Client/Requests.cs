using Metalmynds.BusinessPortalApi.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class Requests
    {

        public static HttpClient Client { get; set; }

        public static BusinessPortalConfiguration Configuration { get; set; }

        private const String GET_SELECTIVE_CALL_RULE_LIST = "/businessportal/customServices/BusinessVoIP/getcallforwardingselectivelist.do";

        private const String SET_SELECTIVE_CALL_RULE_LIST = "/businessportal/customServices/BusinessVoIP/setcallforwardingselectivelist.do";

        private const String SET_SELECTIVE_CALL_RULE = "/businessportal/customServices/BusinessVoIP/setcallforwardingselectiverule.do";

        private const String GET_SELECTIVE_CALL_RULE = "/businessportal/customServices/BusinessVoIP/getcallforwardingselectiverule.do";

        private const String GET_USER_TIME_SCHEDULE_LIST = "/businessportal/customServices/BusinessVoIP/listusertimeschedules.do";

        private const String GET_USER_TIME_SCHEDULE = "/businessportal/customServices/BusinessVoIP/getusertimeschedule.do";


        public async static Task<String> GetSelectiveCallRule(String name)
        {
            var getRuleFields = new Dictionary<String, String>
            {
                { "regKey", Configuration.RegKey },
                { "operation", "modify" },
                { "description", name }
            };

            var getRuleForm = new FormUrlEncodedContent(getRuleFields);

            using (var getRuleResult = await Client.PostAsync(GET_SELECTIVE_CALL_RULE,
                    getRuleForm))
            {
                if (!getRuleResult.IsSuccessStatusCode)
                {
                    throw new PortalClientErrorException("Post", "Selective Call", name);
                }

                return await getRuleResult.Content.ReadAsStringAsync();

            }
        }

        public async static Task SetSelectiveCallRule(SelectiveCallRule rule)
        {
            var fields = Forms.UpdateSelectiveRule(rule);

            fields.Add("regKey", Configuration.RegKey);

            var content = new FormUrlEncodedContent(fields);

            var str = content.ToString();

            using (var response = await Client.PostAsync(SET_SELECTIVE_CALL_RULE,
                    content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new PortalClientErrorException("Post", "Update Selective Rule", rule.Id);
                }
            }
        }

        public async static Task<UserTimeSchedule> GetUserTimeSchedule(String name)
        {

            var query = $"regKey={Configuration.RegKey}&schedule={name}&scheduleType=Personal";

            var response = await Client.GetAsync($"{GET_USER_TIME_SCHEDULE}?{query}");

            if (!response.IsSuccessStatusCode)
            {
                throw new PortalClientErrorException("Get", "User Time Schedule", name);
            }

            var page = await response.Content.ReadAsStringAsync();

            var schedule = Pages.ExtractUserTimeSchedule(page);

            return schedule;
        }

        public async static Task<String> GetUserTimeScheduleList()
        {
            var url = $"{GET_USER_TIME_SCHEDULE_LIST}?regKey={Configuration.RegKey}";

            var response = await Client.GetStringAsync(url);

            return response;
        }

        public async static Task<String> GetSelectiveCallRuleList()
        {
            var url = $"{GET_SELECTIVE_CALL_RULE_LIST}?regKey={Configuration.RegKey}";

            var response = await Client.GetStringAsync(url);

            return response;
        }

        public async static Task CreateSelectiveRule(Dictionary<String, String> fields)
        {

            fields.Add("regKey", Configuration.RegKey);

            var content = new FormUrlEncodedContent(fields);

            using (var response = await Client.PostAsync(GET_SELECTIVE_CALL_RULE, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new PortalClientErrorException("Post", "Create Selective Rule", "");
                }
            }
        }

        public static async Task UpdateSelectiveCallList(Dictionary<String, String> fields)
        {
            fields.Remove("gsRegKey");

            var content = new FormUrlEncodedContent(fields);

            content.Headers.ContentType.CharSet = "UTF-8";

            content.Headers.Add("X-Requested-With", "XMLHttpRequest");            

            using (var response = await Client.PostAsync(SET_SELECTIVE_CALL_RULE_LIST, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new PortalClientErrorException("Post", "Set State Selective Rule", "");
                }
            }

        }
    }

}
