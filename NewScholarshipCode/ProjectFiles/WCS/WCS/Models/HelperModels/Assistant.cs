using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WCS.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WCS.Models
{
    public static class Assistant
    {
        public static Dictionary<string, string> FormRequirementMap = new Dictionary<string, string>{
            { "Current Academic Level", "Text" },
            { "Gender", "Text" },
            { "Marital Status", "Text" },
            { "Overall GPA", "Number" },
            { "Major GPA", "Number" }
        };

        /// <summary>
        /// Returns the current active award cycle
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static AwardCycle GetCurrentAwardCycle(WcsContext context)
        {
            try
            {
                return context.AwardCycles.OrderBy(c => c.StartDate).First(c => c.Status == CycleStatus.Open || c.Status == CycleStatus.Judging);
            }
            catch (Exception)
            {
                return new AwardCycle()
                {
                    Id = 0,
                    CycleName = "No Active Award Cycle",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Status = CycleStatus.Closed
                };
            }
        }

        /// <summary>
        /// Returns the award cycle previous to the one passed in
        /// </summary>
        /// <param name="refCycle"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static AwardCycle GetPreviousAwardCycle(AwardCycle refCycle, WcsContext context)
        {
            try
            {
                return context.AwardCycles.OrderBy(c => c.StartDate).Where(c => c.StartDate < refCycle.StartDate && !c.Recycled).First();
            }
            catch (Exception)
            {
                return refCycle;
            }
        }

        public static AwardCycle GetCurrentApplicationCycle(WcsContext context)
        {
            return context.AwardCycles.OrderBy(c => c.StartDate).FirstOrDefault(c => c.Status == CycleStatus.Open);
        }

        public static AwardCycle GetNextAwardCycle(WcsContext context)
        {
            return context.AwardCycles.OrderBy(c => c.StartDate).FirstOrDefault(c => c.Status == CycleStatus.NotStarted && c.EndDate > DateTime.Now);
        }

        public static string GetSettings(SettingName settingName, WcsContext context)
        {
            string sNameStr = "";
            if (settingName == SettingName.FrontPageMessage)
                sNameStr = "Front Page Message";
            if (settingName == SettingName.HelpPageMessage)
                sNameStr = "HelpPageMessage";

            if (sNameStr == "")
                return null;

            Setting stg = context.Settings.FirstOrDefault(s => s.SettingName.Equals(sNameStr));
            if (stg == null)
            {
                stg = new Setting()
                {
                    SettingName = sNameStr,
                    SettingValue = "<h3 class=\"text - info\">The Scholarship Process</h3><ol><li>Submit scholarship applications before the deadline.</li><li>Application review begins after the application period ends.</li><li>You will be contacted if any additional information or an interview is required.</li><li>After review, scholarships will be awarded. You will be contacted by the scholarship office if you have been awarded a scholarship.</li></ol>"
                };

                context.Settings.Add(stg);
                context.SaveChanges();
            }

            if (settingName == SettingName.HelpPageMessage && string.IsNullOrWhiteSpace(stg.SettingValue))
            {
                stg.SettingValue = "<h3 class=\"text - info\">The Scholarship Process</h3><ol><li>Submit scholarship applications before the deadline.</li><li>Application review begins after the application period ends.</li><li>You will be contacted if any additional information or an interview is required.</li><li>After review, scholarships will be awarded. You will be contacted by the scholarship office if you have been awarded a scholarship.</li></ol>";
            }

            return stg == null ? "" : stg.SettingValue;
        }

        public static bool SaveSetting(SettingName settingName, string value, WcsContext context)
        {
            try
            {
                string sNameStr = "";

                if (settingName == SettingName.FrontPageMessage)
                    sNameStr = "Front Page Message";
                if (settingName == SettingName.HelpPageMessage)
                    sNameStr = "HelpPageMessage";

                if (sNameStr == "")
                    return false;

                var setting = context.Settings.FirstOrDefault(s => s.SettingName.Equals(sNameStr));

                if (setting != null)

                    setting.SettingValue = value;
                else
                    context.Settings.Add(
                        new Setting()
                        {
                            SettingName = sNameStr,
                            SettingValue = value
                        });

                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static EmailSettings GetEmailSettings(WcsContext context)
        {
            EmailSettings thing;
            Setting fromContext = context.Settings.FirstOrDefault(s => s.SettingName == "EmailSettings");

            if (fromContext != null)
            {
                thing = JsonConvert.DeserializeObject(fromContext.SettingValue, typeof(EmailSettings)) as EmailSettings;
                return thing;
            }
            else
            {
                thing = new EmailSettings()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSSL = true,
                    UseDefaultCredentials = false,
                    UserName = "weberscholarships@gmail.com",
                    Password = "AdminPassword1",
                    SenderName = "do-not-reply@csscholarships.weber.edu"
                };

                SaveEMailSettings(thing, context);
                return GetEmailSettings(context);
            }
        }

        public static void SaveEMailSettings(EmailSettings setting, WcsContext context)
        {
            if (setting.Password != null && setting.Password.Trim() != "")
            {
                setting.Password = setting.Password.Trim();
                setting.EncryptPassword();
            }

            Setting fromContext = context.Settings.FirstOrDefault(s => s.SettingName == "EmailSettings");

            if (fromContext != null)
            {
                if (setting.Password == null || setting.Password == "")
                {
                    EmailSettings thing = JsonConvert.DeserializeObject(fromContext.SettingValue, typeof(EmailSettings)) as EmailSettings;
                    setting.Password = thing.Password;
                }

                fromContext.SettingValue = JsonConvert.SerializeObject(setting);
            }
            else
            {
                Setting newSetting = new Setting()
                {
                    SettingName = "EmailSettings",
                    SettingValue = JsonConvert.SerializeObject(setting)
                };

                context.Settings.Add(newSetting);
            }

            context.SaveChanges();
        }
    }

    public enum SettingName
    {
        FrontPageMessage,
        HelpPageMessage
    }
}
