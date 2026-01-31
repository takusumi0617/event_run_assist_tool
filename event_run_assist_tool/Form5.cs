using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            //LoadJsonData();
            //InitializeComboBoxes();
        }
        /*
        private Dictionary<string, object> jsonData;

        private void LoadJsonData()
        {
            string filePath = "area.json"; // JSONファイルのパスに変更してください
            string jsonContent = File.ReadAllText(filePath);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonData = serializer.Deserialize<Dictionary<string, object>>(jsonContent);
        }

        private void InitializeComboBoxes()
        {
            comboBox1.Items.Clear();
            var centers = jsonData["centers"] as Dictionary<string, object>;
            foreach (var center in centers)
            {
                var centerDict = center.Value as Dictionary<string, object>;
                comboBox1.Items.Add(centerDict["name"].ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            var centers = jsonData["centers"] as Dictionary<string, object>;
            var selectedCenter = centers.First(center =>
                (center.Value as Dictionary<string, object>)["name"].ToString() == comboBox1.SelectedItem.ToString()).Value as Dictionary<string, object>;
            var children = selectedCenter["children"] as ArrayList;
            var offices = jsonData["offices"] as Dictionary<string, object>;

            foreach (var child in children)
            {
                string childCode = child.ToString();
                if (offices.ContainsKey(childCode))
                {
                    var office = offices[childCode] as Dictionary<string, object>;
                    comboBox2.Items.Add(office["name"].ToString());
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            var offices = jsonData["offices"] as Dictionary<string, object>;
            var selectedOffice = offices.First(office =>
                (office.Value as Dictionary<string, object>)["name"].ToString() == comboBox2.SelectedItem.ToString()).Value as Dictionary<string, object>;
            var selectedOfficeChildren = selectedOffice["children"] as ArrayList; // 選択された都道府県のchildren

            var class10s = jsonData["class10s"] as Dictionary<string, object>;

            foreach (var class10 in class10s)
            {
                var class10Dict = class10.Value as Dictionary<string, object>;
                if (selectedOfficeChildren.Contains(class10.Key))
                {
                    comboBox3.Items.Add(class10Dict["name"].ToString());
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            var class10s = jsonData["class10s"] as Dictionary<string, object>;
            var selectedClass10 = class10s.First(class10 =>
                (class10.Value as Dictionary<string, object>)["name"].ToString() == comboBox3.SelectedItem.ToString()).Value as Dictionary<string, object>;
            var selectedClass10Children = selectedClass10["children"] as ArrayList; // 選択された第一地方区分のchildren

            var class15s = jsonData["class15s"] as Dictionary<string, object>;

            foreach (var class15 in class15s)
            {
                var class15Dict = class15.Value as Dictionary<string, object>;
                if (selectedClass10Children.Contains(class15.Key))
                {
                    comboBox4.Items.Add(class15Dict["name"].ToString());
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox5.Items.Clear();
            var class15s = jsonData["class15s"] as Dictionary<string, object>;
            var selectedClass15 = class15s.First(class15 =>
                (class15.Value as Dictionary<string, object>)["name"].ToString() == comboBox4.SelectedItem.ToString()).Value as Dictionary<string, object>;
            var selectedClass15Children = selectedClass15["children"] as ArrayList; // 選択された第二地方区分のchildren

            var class20s = jsonData["class20s"] as Dictionary<string, object>;

            foreach (var class20 in class20s)
            {
                var class20Dict = class20.Value as Dictionary<string, object>;
                if (selectedClass15Children.Contains(class20.Key))
                {
                    comboBox5.Items.Add(class20Dict["name"].ToString());
                }
            }
        }
        */

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string local_area_path = main.directory() + "\\temp\\area.json";
            try
            {
                if (!File.Exists(local_area_path) || (DateTime.Now - File.GetLastWriteTime(local_area_path)).TotalDays > 7)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(warning.AREA_URL);
                        response.EnsureSuccessStatusCode();
                        using (FileStream fileStream = new FileStream(local_area_path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                        Console.WriteLine("DONE");
                    }
                }
                AreaData areaData = JsonSerializer.Deserialize<AreaData>(File.ReadAllText(local_area_path));
                // textBox1に入力された市区町村名を取得
                string inputCityName = textBox1.Text.Trim();
                // 一致する市区町村名を持つclass20sのエントリをすべて検索
                var matchingCities = areaData.Class20s
                    .Where(c => c.Value.Name == inputCityName)
                    .ToList();
                // listBox1をクリア
                listBox1.Items.Clear();
                if (matchingCities.Any())
                {
                    // 各一致する市区町村について、表示形式を整えたテキストをlistBox1に追加
                    foreach (var cityEntry in matchingCities)
                    {
                        string cityCode = cityEntry.Key;
                        // 都道府県、地方区分を遡って名前を取得
                        string class15Code = cityEntry.Value.Parent;
                        string class15Name = areaData.Class15s[class15Code].Name;
                        string class10Code = areaData.Class15s[class15Code].Parent;
                        string class10Name = areaData.Class10s[class10Code].Name;
                        string officeCode = areaData.Class10s[class10Code].Parent;
                        string officeName = areaData.Offices[officeCode].Name;
                        // listBox1に追加するテキスト
                        string displayText = $"{cityCode}、{officeName}({class10Name},{class15Name}){inputCityName}";
                        listBox1.Items.Add(displayText);
                    }
                }
                else
                {
                    // 一致する市区町村が見つからなかった場合
                    listBox1.Items.Add("該当する市区町村が見つかりませんでした。");
                }
            }
            catch
            {
                Console.WriteLine("FAILURE");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // listBox1で選択された項目を取得
            if (listBox1.SelectedItem != null)
            {
                // 選択された項目のテキストを取得
                string selectedText = listBox1.SelectedItem.ToString();

                // 市区町村コード部分を抽出
                string cityCode = selectedText.Split('、')[0];

                // レジストリに保存
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\erat");
                if (key != null)
                {
                    key.SetValue("area", cityCode);
                    key.Close();
                    MessageBox.Show("市区町村コードがレジストリに保存されました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("リストから市区町村を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // JSONデータを保存するクラスを作成
    public class AreaData
    {
        [JsonPropertyName("centers")]
        public Dictionary<string, Center> Centers { get; set; }

        [JsonPropertyName("offices")]
        public Dictionary<string, Office> Offices { get; set; }

        [JsonPropertyName("class10s")]
        public Dictionary<string, Class10> Class10s { get; set; }

        [JsonPropertyName("class15s")]
        public Dictionary<string, Class15> Class15s { get; set; }

        [JsonPropertyName("class20s")]
        public Dictionary<string, Class20> Class20s { get; set; }
    }

    public class Center
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("officeName")]
        public string OfficeName { get; set; }

        [JsonPropertyName("children")]
        public List<string> Children { get; set; }
    }

    public class Office
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("officeName")]
        public string OfficeName { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

        [JsonPropertyName("children")]
        public List<string> Children { get; set; }
    }

    public class Class10
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

        [JsonPropertyName("children")]
        public List<string> Children { get; set; }
    }

    public class Class15
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

        [JsonPropertyName("children")]
        public List<string> Children { get; set; }
    }

    public class Class20
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }
    }
}
