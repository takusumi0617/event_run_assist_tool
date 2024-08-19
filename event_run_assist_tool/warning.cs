using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_run_assist_tool
{
    class warning
    {
        const string CLASS_AREA_CODE = "1310100"; // 市町村区のコード
        const string AREA_URL = "https://www.jma.go.jp/bosai/common/const/area.json";
        static string warning_info_url = $"https://www.jma.go.jp/bosai/warning/#area_type=class20s&area_code={CLASS_AREA_CODE}&lang=ja";
        static string url = "https://www.jma.go.jp/bosai/warning/data/warning/{0}.json";

        static async Task<(List<string> warningTexts, string area)> GetWarningsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // コードと名称の取得
                string warningInfoHtml = await client.GetStringAsync(warning_info_url);
                HtmlDocument warningsDoc = new HtmlDocument();
                warningsDoc.LoadHtml(warningInfoHtml);
                var warningsContents = warningsDoc.DocumentNode.SelectNodes("//script")[10].InnerText;
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
                dynamic areaData = JsonConvert.DeserializeObject(areaDataJson);
                string area = areaData["class20s"][CLASS_AREA_CODE]["name"];
                string class15sAreaCode = areaData["class20s"][CLASS_AREA_CODE]["parent"];
                string class10sAreaCode = areaData["class15s"][class15sAreaCode]["parent"];
                string officesAreaCode = areaData["class10s"][class10sAreaCode]["parent"];
                string warningInfoJson = await client.GetStringAsync(string.Format(url, officesAreaCode));
                dynamic warningInfo = JsonConvert.DeserializeObject(warningInfoJson);

                List<string> warningCodes = new List<string>();
                foreach (var classArea in warningInfo["areaTypes"][1]["areas"])
                {
                    if (classArea["code"] == CLASS_AREA_CODE)
                    {
                        foreach (var warning in classArea["warnings"])
                        {
                            if (warning["status"] != "解除" && warning["status"] != "発表警報・注意報はなし")
                            {
                                warningCodes.Add((string)warning["code"]);
                            }
                        }
                    }
                }

                List<string> warningTexts = new List<string>();
                foreach (string code in warningCodes)
                {
                    if (transWarning.ContainsKey(code))
                    {
                        warningTexts.Add(transWarning[code]);
                    }
                }

                return (warningTexts, area);
            }
        }
    }
}
