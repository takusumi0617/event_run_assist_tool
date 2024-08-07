using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Xml.Linq;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace event_run_assist_tool
{
    public partial class Form3 : Form
    {
        string path;
        int sum = 0;
        int todaysum = 0;
        int rank = -1;
        private SQLiteConnection _connection;
        private TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
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
                string createTable1Query = @"
            CREATE TABLE IF NOT EXISTS point (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                time DATETIME NOT NULL,
                type INTEGER NOT NULL,
                value INTEGER NOT NULL,
                sum INTEGER NOT NULL,
                comment TEXT
            );";
                string createTable2Query = @"
            CREATE TABLE IF NOT EXISTS rank (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                time DATETIME NOT NULL,
                value INTEGER NOT NULL,
                comment TEXT
            );";
                using (SQLiteCommand command = new SQLiteCommand(createTable1Query, _connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(createTable2Query, _connection))
                {
                    command.ExecuteNonQuery();
                }
                timer1.Enabled = true;
                timer2.Enabled = true;
                timer1_Tick(sender, e);
                timer2_Tick(sender, e);
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
            if (radioButton1.Checked)
            {
                // 最新のsum値を取得するSQLクエリ
                string getLastSumQuery = "SELECT sum FROM point ORDER BY id DESC LIMIT 1";

                int lastSum = 0;
                using (SQLiteCommand command = new SQLiteCommand(getLastSumQuery, _connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        lastSum = Convert.ToInt32(result);
                    }
                }

                // 新しいデータを挿入するSQLクエリ
                string insertDataQuery = "INSERT INTO point (time, type, value, sum) VALUES (@time, @type, @value, @sum)";

                using (SQLiteCommand command = new SQLiteCommand(insertDataQuery, _connection))
                {
                    command.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@type", 1);
                    command.Parameters.AddWithValue("@value", int.Parse(textBox1.Text));
                    command.Parameters.AddWithValue("@sum", lastSum + int.Parse(textBox1.Text));
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                string insertDataQuery = "INSERT INTO point (time, type, value, sum, comment) VALUES (@time, @type, @value, @sum, @comment);";

                using (SQLiteCommand command = new SQLiteCommand(insertDataQuery, _connection))
                {
                    command.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@type", 2);
                    command.Parameters.AddWithValue("@value", int.Parse(textBox1.Text));
                    command.Parameters.AddWithValue("@sum", int.Parse(textBox1.Text));
                    command.Parameters.AddWithValue("@comment", "置換処理");
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show("データを追加しました。", "データベース更新完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox1.Text = string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string insertDataQuery = "INSERT INTO rank (time, value) VALUES (@time, @value);";

            using (SQLiteCommand command = new SQLiteCommand(insertDataQuery, _connection))
            {
                command.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@value", int.Parse(textBox2.Text));
                command.ExecuteNonQuery();
            }
            MessageBox.Show("データを追加しました。", "データベース更新完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox2.Text = string.Empty;
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
            //残りやること:「本日の総ポイント」の目標値の計算式実装
            TimeSpan starttonow = DateTime.Now - DateTime.Parse(dic["starttime"]);
            string getLastSumQuery = "SELECT sum FROM point ORDER BY id DESC LIMIT 1;";
            using (SQLiteCommand command = new SQLiteCommand(getLastSumQuery, _connection))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    sum = reader.GetInt32(0); // sum列の値を取得
                    if (sum >= int.Parse(dic["goal"])) label11.ForeColor = System.Drawing.Color.Green;
                    else label11.ForeColor = Color.FromName("ControlText");
                }
                else sum = 0;
            }
            label11.Text = sum.ToString();
            label16.Text = (sum / (decimal)starttonow.TotalHours).ToString("F");
            {
                // 今日の日付の最後のsum値を取得
                DateTime today = DateTime.Today.ToUniversalTime();
                DateTime tomorrow = today.AddDays(1);
                string query = @"
                    SELECT sum 
                    FROM point 
                    WHERE time >= @today AND time < @tomorrow
                    ORDER BY time DESC
                    LIMIT 1";

                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@today", today.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@tomorrow", tomorrow.ToString("yyyy-MM-dd"));
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        todaysum = Convert.ToInt32(result);
                        label22.Text = todaysum.ToString();
                    }
                    else
                    {
                        todaysum = 0;
                        label22.Text = "0";
                    }
                }
            }
            {
                TimeSpan todaytonow = DateTime.Now - DateTime.Today;
                double totalHours = todaytonow.TotalHours;
                label25.Text = (todaysum / totalHours).ToString("F");
            }
            {
                string query = @"
                    SELECT value 
                    FROM rank 
                    ORDER BY time DESC 
                    LIMIT 1";

                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        rank = Convert.ToInt32(result);
                        label19.Text = rank.ToString();
                        if (rank <= int.Parse(dic["rank"])) label19.ForeColor = System.Drawing.Color.Green;
                        else label19.ForeColor = Color.FromName("ControlText");
                    }
                    else
                    {
                        label19.Text = "None";
                    }
                }
            }
            {
                DateTime startTime = DateTime.Parse(dic["starttime"]);
                DateTime endTime = DateTime.Parse(dic["endtime"]);

                // 全日付を取得する
                var dates = Enumerable.Range(0, (endTime - startTime).Days + 1)
                                      .Select(offset => startTime.AddDays(offset).Date)
                                      .ToList();
                string query = @"
            WITH RankedData AS (
                SELECT
                    DATE(time) AS date,
                    SUM(sum) AS sum,
                    ROW_NUMBER() OVER (PARTITION BY DATE(time) ORDER BY time DESC) AS rn
                FROM
                    point
                WHERE
                    DATE(time) BETWEEN @startDate AND @endDate
                GROUP BY
                    DATE(time)
            )
            SELECT
                date,
                sum
            FROM
                RankedData
            WHERE
                rn = 1
            ORDER BY
                date;
        ";

                using (var command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@startDate", startTime.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@endDate", endTime.ToString("yyyy-MM-dd"));
                    var dataTable = new DataTable();
                    dataTable.Load(command.ExecuteReader());

                    var series = new Series("Sum")
                    {
                        ChartType = SeriesChartType.Area,
                        Name = "総ポイント数",
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 8,
                        MarkerColor = System.Drawing.Color.Blue
                    };
                    chart1.Series.Clear();
                    chart1.Series.Add(series);

                    var dataDict = new Dictionary<DateTime, int>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        DateTime timeUtc;
                        int value;

                        if (DateTime.TryParse(row["date"].ToString(), out timeUtc) &&
                            int.TryParse(row["sum"].ToString(), out value))
                        {
                            // UTCからローカルタイムに変換
                            DateTime timeLocal = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);
                            series.Points.AddXY(timeLocal, value);
                        }
                    }

                    foreach (var date in dates)
                    {
                        var sum = dataDict.ContainsKey(date) ? dataDict[date] : 0;
                        series.Points.AddXY(date, sum);
                    }
                    var chartArea = chart1.ChartAreas[0];
                    chartArea.AxisX.Title = "日付";
                    chartArea.AxisY.Title = "総ポイント(Pt)";
                    // 縦軸の上下を逆転
                    chartArea.AxisY.IsReversed = true;

                    // 横軸の目盛りラベルを90°回転
                    chartArea.AxisX.LabelStyle.Angle = 90;

                    // 軸の太さと目盛り線の色を設定
                    chartArea.AxisX.LineWidth = 2; // 横軸の線の太さ
                    chartArea.AxisY.LineWidth = 2; // 縦軸の線の太さ

                    chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray; // 横軸の目盛り線の色
                    chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray; // 縦軸の目盛り線の色
                    chartArea.AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray; // 横軸の補助目盛り線の色
                    chartArea.AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray; // 縦軸の補助目盛り線の色

                    // goal の値を int に変換
                    int goal = int.Parse(dic["goal"]);

                    // 目標値を示す直線を追加
                    var goalLine = new Series("Goal")
                    {
                        ChartType = SeriesChartType.Line,
                        BorderDashStyle = ChartDashStyle.Dash,
                        Color = System.Drawing.Color.Red,
                        Name = "目標ポイント"
                    };

                    // 目標値の直線を描画するために2つのポイントを追加
                    goalLine.Points.AddXY(startTime, goal);
                    goalLine.Points.AddXY(endTime, goal);

                    chart1.Series.Add(goalLine);
                }
            }
            {
                DateTime startTime = DateTime.Parse(dic["starttime"]);
                DateTime endTime = DateTime.Parse(dic["endtime"]);
                string query = @"
            SELECT
                time,
                value
            FROM
                rank
            WHERE
                DATE(time) BETWEEN @startDate AND @endDate
            ORDER BY
                time;
        ";

                using (var command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@startDate", startTime.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@endDate", endTime.ToString("yyyy-MM-dd"));

                    var dataTable = new DataTable();
                    dataTable.Load(command.ExecuteReader());

                    var series = new Series("Rank")
                    {
                        ChartType = SeriesChartType.Line,
                        BorderDashStyle = ChartDashStyle.Solid,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 8,
                        MarkerColor = System.Drawing.Color.Blue
                    };
                    chart2.Series.Clear();
                    chart2.Series.Add(series);

                    // データをプロット
                    foreach (DataRow row in dataTable.Rows)
                    {
                        DateTime timeUtc;
                        int value;

                        if (DateTime.TryParse(row["time"].ToString(), out timeUtc) &&
                            int.TryParse(row["value"].ToString(), out value))
                        {
                            // UTCからローカルタイムに変換
                            DateTime timeLocal = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);
                            series.Points.AddXY(timeLocal, value);
                        }
                    }

                    var chartArea = chart2.ChartAreas[0];
                    chartArea.AxisX.Title = "Date";
                    chartArea.AxisY.Title = "Rank";

                    // 縦軸の上下を逆転
                    chartArea.AxisY.IsReversed = true;

                    // 横軸の目盛りラベルを90°回転
                    chartArea.AxisX.LabelStyle.Angle = 90;

                    // 軸の太さと目盛り線の色を設定
                    chartArea.AxisX.LineWidth = 2; // 横軸の線の太さ
                    chartArea.AxisY.LineWidth = 2; // 縦軸の線の太さ

                    chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray; // 横軸の目盛り線の色
                    chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray; // 縦軸の目盛り線の色
                    chartArea.AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray; // 横軸の補助目盛り線の色
                    chartArea.AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray; // 縦軸の補助目盛り線の色

                    // 目標値を示す直線を追加（例として1位）
                    int goal = int.Parse(dic["rank"]);
                    var goalLine = new Series("Goal")
                    {
                        ChartType = SeriesChartType.Line,
                        BorderDashStyle = ChartDashStyle.Dash,
                        Color = System.Drawing.Color.Red
                    };

                    goalLine.Points.AddXY(startTime, goal);
                    goalLine.Points.AddXY(endTime, goal);

                    chart2.Series.Add(goalLine);
                }
            }
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

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            timer1.Enabled = false;
            timer2.Enabled = false;
        }
    }
}
