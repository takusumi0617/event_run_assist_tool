using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class Form3 : Form
    {
        string path;
        int sum = 0;
        private SQLiteConnection _connection;
        Dictionary<string, string> dic = new Dictionary<string, string> { };
        /*{
            { "goal", ""}, //目標ポイント数
            { "rank", ""}, //目標順位
            { "bonus",  ""}, //合計ライボ数
            { "speed", ""}, //焚き数
            { "pppA", ""}, //一回当たりポイント数(0焚き時)
            { "starttime", ""}, //開始日
            { "endtime", ""}, //終了日
            { "tpp", ""}, //一回当たりの時間
            { "pppB", ""}, //一回当たりポイント数(焚き数反映済み)
            { "frequency", ""}, //合計ライブ回数
            { "totalpoint", ""} //合計予想ポイント数
        }*/
        public Form3(string path1)
        {
            InitializeComponent();
            path = path1;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    dic = JsonSerializer.Deserialize<Dictionary<string, string>>(json, GetOption());
                    label13.Text = dic["goal"];
                    label17.Text = dic["rank"];
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("エラーが発生しました。\r\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string datapath = main.directory() + "\\database\\" + Path.GetFileNameWithoutExtension(path) + ".db";
                string connectionString = $"Data Source={datapath}";
                Batteries.Init();
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                // テーブルを作成
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS data (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                time DATETIME NOT NULL,
                type INTEGER NOT NULL,
                value INTEGER NOT NULL,
                sum INTEGER NOT NULL,
                comment TEXT
            );";

                using (SQLiteCommand command = new SQLiteCommand(createTableQuery, _connection))
                {
                    command.ExecuteNonQuery();
                }
                timer1.Enabled = true;
                timer2.Enabled = true;
            }
            else
            {
                MessageBox.Show("パスが存在しません。", "引数エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            TimeSpan ts = DateTime.Parse(dic["endtime"]) - DateTime.Now;
            if (!(ts.Days <= 0 && ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0))
            {
                label4.Text = $"{ts.Days}日{ts.Hours}時間{ts.Minutes}分{ts.Seconds}秒";
                if (ts.Days <= 0) label4.ForeColor = System.Drawing.Color.Red;
                else label4.ForeColor = Color.FromName("ControlText");
                if (ts.Days <= 0 && ts.Hours <= 0) label4.Font = new Font(label4.Font, FontStyle.Bold);
            }
            else
            {
                label4.Text = "終了";
                label4.ForeColor = System.Drawing.Color.Red;
                label4.Font = new Font(label4.Font, FontStyle.Bold);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string getLastSumQuery = "SELECT sum FROM data ORDER BY id DESC LIMIT 1;";
            using (SQLiteCommand command = new SQLiteCommand(getLastSumQuery, _connection))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    sum = reader.GetInt32(0); // sum列の値を取得
                }
                else
                {
                    sum = 0;
                }
            }
            label11.Text = sum.ToString();
        }

        private static JsonSerializerOptions GetOption()
        {
            // ユニコードのレンジ指定で日本語も正しく表示、インデントされるように指定
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return options;
        }

        private void InsertData(DateTime time, int type, int value, int sum, string comment)
        {
            string insertDataQuery = "INSERT INTO data (time, type, value, sum, comment) VALUES (@time, @type, @value, @sum, @comment);";

            using (SQLiteCommand command = new SQLiteCommand(insertDataQuery, _connection))
            {
                command.Parameters.AddWithValue("@time", time);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@value", value);
                command.Parameters.AddWithValue("@sum", sum);
                command.Parameters.AddWithValue("@comment", comment);
                command.ExecuteNonQuery();
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}
