using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace event_run_assist_tool
{
    class warning
    {
        public const string CLASS_AREA_CODE = "1310100"; // 市町村区のコード
        const string AREA_URL = "https://www.jma.go.jp/bosai/common/const/area.json";
        static string warning_info_url = $"https://www.jma.go.jp/bosai/warning/#area_type=class20s&area_code={CLASS_AREA_CODE}&lang=ja";
        static string url = "https://www.jma.go.jp/bosai/warning/data/warning/{0}.json";

        public static async Task<(List<string> warningTexts, string area)> GetWarningsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // コードと名称の取得
                string warningInfoHtml = await client.GetStringAsync(warning_info_url);
                var warningsContents = GetWarningsScriptContent(warningInfoHtml);
                var warningsTextList = warningsContents.Split(new[] { "}," }, StringSplitOptions.None);

                Dictionary<string, string> transWarning = new Dictionary<string, string>();
                foreach (var warning in warningsTextList)
                {
                    if (warning.Contains(":{name1:\""))
                    {
                        var warningData = Regex.Matches(warning, @"\w+");
                        string warningText = "";
                        string warningCode = "";
                        foreach (Match match in warningData)
                        {
                            string warningDatum = match.Value;
                            if (warningDatum == "elem")
                                break;
                            if (new[] { "c", "name1", "name2" }.Contains(warningDatum))
                                continue;
                            if (int.TryParse(warningDatum, out _))
                            {
                                warningCode = warningDatum;
                                continue;
                            }
                            warningText += "\\" + warningDatum;
                        }
                        transWarning[warningCode] = Regex.Unescape(warningText);
                    }
                }

                // 情報の取得
                string areaDataJson = await client.GetStringAsync(AREA_URL);
                JsonDocument areaData = JsonDocument.Parse(areaDataJson);
                string area = areaData.RootElement.GetProperty("class20s").GetProperty(CLASS_AREA_CODE).GetProperty("name").GetString();
                string class15sAreaCode = areaData.RootElement.GetProperty("class20s").GetProperty(CLASS_AREA_CODE).GetProperty("parent").GetString();
                string class10sAreaCode = areaData.RootElement.GetProperty("class15s").GetProperty(class15sAreaCode).GetProperty("parent").GetString();
                string officesAreaCode = areaData.RootElement.GetProperty("class10s").GetProperty(class10sAreaCode).GetProperty("parent").GetString();
                string warningInfoJson = await client.GetStringAsync(string.Format(url, officesAreaCode));
                JsonDocument warningInfo = JsonDocument.Parse(warningInfoJson);

                List<string> warningCodes = new List<string>();
                foreach (var classArea in warningInfo.RootElement.GetProperty("areaTypes")[1].GetProperty("areas").EnumerateArray())
                {
                    if (classArea.GetProperty("code").GetString() == CLASS_AREA_CODE)
                    {
                        foreach (var warning in classArea.GetProperty("warnings").EnumerateArray())
                        {
                            if (warning.GetProperty("status").GetString() != "解除" && warning.GetProperty("status").GetString() != "発表警報・注意報はなし")
                            {
                                warningCodes.Add(warning.GetProperty("code").GetString());
                            }
                        }
                    }
                }

                List<string> warningTexts = warningCodes.Select(code => transWarning.ContainsKey(code) ? transWarning[code] : "").ToList();
                return (warningTexts, area);
            }
        }

        static string GetWarningsScriptContent(string html)
        {
            string pattern = @"<script.*?>.*?</script>";
            var matches = Regex.Matches(html, pattern, RegexOptions.Singleline);
            return matches.Count > 10 ? matches[10].Value : string.Empty;
        }
    }
}
