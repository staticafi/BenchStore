using System.Text.RegularExpressions;

using BenchStoreBL.XMLData;

namespace BenchStoreBL.Models.Mappers
{
    public static class XMLElementMapper
    {
        private static Dictionary<string, string> timeZoneOffsets =
            new Dictionary<string, string>() {
                {"ACDT", "+10:30"},
                {"ACST", "+09:30"},
                {"ADT", "-03:00"},
                {"AEDT", "+11:00"},
                {"AEST", "+10:00"},
                {"AHDT", "-09:00"},
                {"AHST", "-10:00"},
                {"AST", "-04:00"},
                {"AT", "-02:00"},
                {"AWDT", "+09:00"},
                {"AWST", "+08:00"},
                {"BAT", "+03:00"},
                {"BDST", "+02:00"},
                {"BET", "-11:00"},
                {"BST", "-03:00"},
                {"BT", "+03:00"},
                {"BZT2", "-03:00"},
                {"CADT", "+10:30"},
                {"CAST", "+09:30"},
                {"CAT", "-10:00"},
                {"CCT", "+08:00"},
                {"CDT", "-05:00"},
                {"CED", "+02:00"},
                {"CET", "+01:00"},
                {"CEST", "+02:00"},
                {"CST", "-06:00"},
                {"EAST", "+10:00"},
                {"EDT", "-04:00"},
                {"EED", "+03:00"},
                {"EET", "+02:00"},
                {"EEST", "+03:00"},
                {"EST", "-05:00"},
                {"FST", "+02:00"},
                {"FWT", "+01:00"},
                {"GMT", "+00:00"},
                {"GST", "+10:00"},
                {"HDT", "-09:00"},
                {"HST", "-10:00"},
                {"IDLE", "+12:00"},
                {"IDLW", "-12:00"},
                {"IST", "+05:30"},
                {"IT", "+03:30"},
                {"JST", "+09:00"},
                {"JT", "+07:00"},
                {"MDT", "-06:00"},
                {"MED", "+02:00"},
                {"MET", "+01:00"},
                {"MEST", "+02:00"},
                {"MEWT", "+01:00"},
                {"MST", "-07:00"},
                {"MT", "+08:00"},
                {"NDT", "-02:30"},
                {"NFT", "-03:30"},
                {"NT", "-11:00"},
                {"NST", "+06:30"},
                {"NZ", "+11:00"},
                {"NZST", "+12:00"},
                {"NZDT", "+13:00"},
                {"NZT", "+12:00"},
                {"PDT", "-07:00"},
                {"PST", "-08:00"},
                {"ROK", "+09:00"},
                {"SAD", "+10:00"},
                {"SAST", "+09:00"},
                {"SAT", "+09:00"},
                {"SDT", "+10:00"},
                {"SST", "+02:00"},
                {"SWT", "+01:00"},
                {"USZ3", "+04:00"},
                {"USZ4", "+05:00"},
                {"USZ5", "+06:00"},
                {"USZ6", "+07:00"},
                {"UT", "-00:00"},
                {"UTC", "-00:00"},
                {"UZ10", "+11:00"},
                {"WAT", "-01:00"},
                {"WET", "-00:00"},
                {"WST", "+08:00"},
                {"YDT", "-08:00"},
                {"YST", "-09:00"},
                {"ZP4", "+04:00"},
                {"ZP5", "+05:00"},
                {"ZP6", "+06:00"}
            };

        public static Result MapToModel(this XMLResultElement xmlResultElement)
        {
            long timeLimit = 0;
            long memLimit = 0;

            if (xmlResultElement.TimeLimit != null)
            {
                timeLimit = GetTimeLimit(xmlResultElement.TimeLimit);
            }

            if (xmlResultElement.MemLimit != null)
            {
                memLimit = GetMemLimit(xmlResultElement.MemLimit);
            }

            DateTime startTime = xmlResultElement.StartTime.ToUniversalTime();
            DateTime endTime = xmlResultElement.EndTime.ToUniversalTime();
            DateTime date = GetDate(xmlResultElement.Date).ToUniversalTime();
            return new Result
            {
                Name = xmlResultElement.Name,
                BenchmarkName = xmlResultElement.BenchmarkName,
                Block = xmlResultElement.Block,
                CPUCores = xmlResultElement.CPUCores,
                Date = date,
                DisplayName = xmlResultElement.DisplayName,
                StartTime = startTime,
                EndTime = endTime,
                Options = xmlResultElement.Options,
                TimeLimit = timeLimit,
                MemLimit = memLimit,
                Tool = xmlResultElement.Tool,
                ToolModule = xmlResultElement.ToolModule,
                Version = xmlResultElement.Version,
                Error = xmlResultElement.Error,
                Generator = xmlResultElement.Generator,
            };
        }

        private static long GetMemLimit(string memLimitInput)
        {
            Regex memoryRegex = new Regex(@"^\d+B$");
            long memLimit;
            if (memoryRegex.IsMatch(memLimitInput))
            {
                string memLimitStripped = Regex.Replace(memLimitInput, @"B$", "");
                if (!long.TryParse(memLimitStripped, out memLimit))
                {
                    throw new ArgumentException($"Failed to parse {nameof(Result.MemLimit)}: '{memLimitStripped}' as {nameof(Decimal)}!");
                }
            }
            else
            {
                throw new ArgumentException($"Element {nameof(Result)} field: {nameof(Result.MemLimit)}: '{memLimitInput}' is of the wrong format!");
            }

            return memLimit;
        }

        private static long GetTimeLimit(string timeLimitInput)
        {
            Regex timeRegex = new Regex(@"^\d+s$");
            long timeLimit = 0;
            if (timeRegex.IsMatch(timeLimitInput))
            {
                string timeLimitStripped = Regex.Replace(timeLimitInput, @"s$", "");
                if (!long.TryParse(timeLimitStripped, out timeLimit))
                {
                    throw new ArgumentException($"Failed to parse {nameof(Result.TimeLimit)}: '{timeLimitStripped}' as {nameof(Decimal)}!");
                }
            }
            else
            {
                throw new ArgumentException($"Element {nameof(Result)} field: {nameof(Result.TimeLimit)}: '{timeLimitInput}' is of the wrong format!");
            }

            return timeLimit;
        }

        private static DateTime GetDate(string date)
        {
            int timezonePosition = date.LastIndexOf(' ') + 1;
            string timezone = date.Substring(timezonePosition);

            string parsedDate = date.Replace(timezone, timeZoneOffsets[timezone]);

            return DateTime.ParseExact(parsedDate, "yyyy-MM-dd HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

