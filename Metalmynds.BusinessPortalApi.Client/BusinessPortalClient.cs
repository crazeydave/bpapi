using Metalmynds.BusinessPortalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metalmynds.BusinessPortalApi.Client
{
    public class BusinessPortalClient
    {
        protected HttpClient _client;

        protected BusinessPortalConfiguration _configuration;

        protected DateTime _lastSuccessfulLogin;

        public BusinessPortalClient(BusinessPortalConfiguration configuration, HttpClient client)
        {

            _configuration = configuration;

            if (_client == null)
            {
                _client = new HttpClient();

            }

            _client = client;

            _configuration = configuration;

            _client.BaseAddress = new Uri(_configuration.BaseUrl);

            _client.DefaultRequestHeaders.Add("Bpapi", "1.0");

            Requests.Client = _client;

            Requests.Configuration = _configuration;
        }


        protected async Task Login()
        {

            var sinceLogin = DateTime.Now.Subtract(_lastSuccessfulLogin);

            if (sinceLogin.TotalMinutes > _configuration.TimeoutMinutes)
            {

                using (var landing = await _client.GetAsync($"/businessportal/customServices/BusinessVoIP/getuserdashboard.do?{_configuration.RegKey}"))
                {

                    if (landing.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {

                        using (var loginForm = new FormUrlEncodedContent(new Dictionary<string, string>() {
                            { "shouldSetDomain", _configuration.ShouldSetDomain.ToString().ToLower() },
                            { "timeZoneOffset", _configuration.TimeZoneOffset.ToString() },
                            { "name", _configuration.User },
                            { "domain", _configuration.Domain },
                            { "password", _configuration.Password }}))
                            {
                                using (var login = await _client.PostAsync("/businessportal/login.do", loginForm))
                                {
                                    if (!login.IsSuccessStatusCode)
                                    {
                                        throw new BusinessPortalLoginFailedException(_configuration.Domain, _configuration.User, new string('*', _configuration.Password.Length));
                                    }
                                    else
                                    {
                                        _lastSuccessfulLogin = DateTime.Now;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _lastSuccessfulLogin = DateTime.Now;
                        }
                    }

            }

        }

        protected async Task<SelectiveCallRule> GetSelectiveCallRule(String name, Boolean enabled)
        {
            await Login();

            var response = await Requests.GetSelectiveCallRule(name);

            var rule = Pages.ExtractSelectiveCallRule(response);

            rule.Enabled = enabled;

            return rule;
        }

        public async Task<SelectiveCallRule> GetSelectiveCallRule(String name)
        {
            await Login();

            var response = await Requests.GetSelectiveCallRule(name);

            var rule = Pages.ExtractSelectiveCallRule(response);

            var list = await Requests.GetSelectiveCallRuleList();

            var fields = Pages.GetFormFields(list);

            rule.Enabled = Forms.IsSelectiveRuleEnabled(rule.Id, fields);

            return rule;
        }
   
        public async Task<UserTimeSchedule> GetUserTimeSchedule(String name)
        {
            await Login();

            var user = await Requests.GetUserTimeSchedule(name);

            return user;

        }

        public async Task<List<SelectiveCallRule>> GetSelectiveCallRules()
        {
            await Login();

            var list = await Requests.GetSelectiveCallRuleList();

            var fields = Pages.GetFormFields(list);

            var names = from field in fields where field.Key.StartsWith("description") select field.Value;

            var getRules = new List<Task<SelectiveCallRule>>();

            names.ToList().ForEach((name) =>
            {
                var enabled = Forms.IsSelectiveRuleEnabled(name, fields);

                getRules.Add(GetSelectiveCallRule(name, enabled));

            });

            Task.WaitAll(getRules.ToArray());

            var rules = new List<SelectiveCallRule>();

            getRules.ForEach((getRule) =>
            {

                rules.Add(getRule.Result);

            });

            return rules;

        }

        public async Task<List<UserTimeSchedule>> GetUserTimeSchedules()
        {
            await Login();

            var list = await Requests.GetUserTimeScheduleList();

            var fields = Pages.GetUserTimeScheduleListFields(list);

            var names = from item in fields select item.Value;

            var getSchedules = new List<Task<UserTimeSchedule>>();

            names.ToList().ForEach((name) =>
            {

                getSchedules.Add(GetUserTimeSchedule(name));

            });

            Task.WaitAll(getSchedules.ToArray());

            var schedules = new List<UserTimeSchedule>();

            getSchedules.ForEach((getTask) =>
            {

                var schedule = getTask.Result;

                schedules.Add(schedule);

            });

            return schedules;

        }

        public async Task CreateSelectiveCallRule(SelectiveCallRule rule)
        {
            await Login();

            var fields = Forms.CreateSelectiveRule(rule);

            await Requests.CreateSelectiveRule(fields);

            await UpdateSelectiveCallRuleState(rule);

        }

        protected async Task UpdateSelectiveCallRuleState(SelectiveCallRule rule)
        {
            var list = await Requests.GetSelectiveCallRuleList();

            var raw = Pages.GetFormFields(list);

            // For Sanity Remove All Delete Rule Fields 
            var fields = Forms.RemoveFields(new String[] { "deleteRule" } , raw);

            var update = false;

            if (!rule.Enabled && Forms.IsSelectiveRuleEnabled(rule.Id, fields))
            {
                Forms.DisableSelectiveRule(rule.Id, fields);
                update = true;
            }
            else if (rule.Enabled && !Forms.IsSelectiveRuleEnabled(rule.Id, fields))
            {
                Forms.EnableSelectiveRule(rule.Id, fields);
                update = true;
            }

            if (update)
            {                
                await Requests.UpdateSelectiveCallList(fields);
            }
        }

        public async Task UpdateSelectiveCallRule(String id, SelectiveCallRule updated)
        {
            await Login();

            var existing = await GetSelectiveCallRule(id);

            updated.Name =
                    (updated.Name != existing.Name
                    ? updated.Name : existing.Name);

            updated.AcceptCalls =
                    (updated.AcceptCalls != existing.AcceptCalls
                    ? updated.AcceptCalls : existing.AcceptCalls);

            updated.Forward =
                    (updated.Forward != existing.Forward
                    ? updated.Forward : existing.Forward);

            updated.AcceptUnknownNumbers =
                    (updated.AcceptUnknownNumbers != existing.AcceptUnknownNumbers
                    ? updated.AcceptUnknownNumbers : existing.AcceptUnknownNumbers);

            updated.AcceptPrivateNumbers =
                    (updated.AcceptPrivateNumbers != existing.AcceptPrivateNumbers
                    ? updated.AcceptPrivateNumbers : existing.AcceptPrivateNumbers);

            updated.TimeSchedule =
                    (updated.TimeSchedule != existing.TimeSchedule
                    ? updated.TimeSchedule : existing.TimeSchedule);

            updated.HolidaySchedule =
                    (updated.HolidaySchedule != existing.HolidaySchedule
                    ? updated.HolidaySchedule : existing.HolidaySchedule);

            updated.PhoneNumberOrSipUrl =
                    (updated.PhoneNumberOrSipUrl != existing.PhoneNumberOrSipUrl
                    ? updated.PhoneNumberOrSipUrl : existing.PhoneNumberOrSipUrl);

            await Requests.SetSelectiveCallRule(updated);

            // If we have renamed rule.
            updated.Id = updated.Name;

            await UpdateSelectiveCallRuleState(updated);

        }

        public async Task DeleteSelectiveCallRule(SelectiveCallRule rule)
        {
            await Login();

            throw new NotImplementedException();
        }

    }

}