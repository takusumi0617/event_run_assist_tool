using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rank = 100;
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    rank = 1;
                    break;
                case 1:
                    rank = 50;
                    break;
                case 2:
                    rank = 100;
                    break;
                case 3:
                    rank = 200;
                    break;
                case 4:
                    rank = 500;
                    break;
                case 5:
                    rank = 1000;
                    break;
                default:
                    rank = 100;
                    break;
            }
            int bonus = int.Parse(textBox2.Text) + int.Parse(textBox6.Text) + (10 * (int.Parse(textBox3.Text) + int.Parse(textBox4.Text))) + (int.Parse(textBox4.Text) / 10);
            int ppt = (int.Parse(textBox8.Text) * magnificate(int.Parse(textBox7.Text)));
            int frequency = bonus / int.Parse(textBox7.Text);

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "goal", textBox1.Text}, //目標ポイント数
                { "rank", rank.ToString()}, //目標順位
                { "bonus",  bonus.ToString()}, //合計ライボ数
                { "speed", textBox7.Text}, //焚き数
                { "pppA", textBox8.Text}, //一回当たりポイント数(0焚き時)
                { "starttime", dateTimePicker1.Value.ToString()}, //開始日
                { "endtime", dateTimePicker2.Value.ToString()}, //終了日
                { "tpp", textBox10.Text}, //一回当たりの時間
                { "pppB", ppt.ToString()}, //一回当たりポイント数(焚き数反映済み)
                { "frequency", frequency.ToString()}, //合計ライブ回数
                { "totalpoint", (ppt * frequency).ToString()} //合計予想ポイント数
            };

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "event_run_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = main.directory() + @"\data";
            //[ファイルの種類]に表示される選択肢を指定する
            sfd.Filter = "JSONファイル(*.json)|*.json|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //1番目の「JSONファイル」が選択されているようにする
            sfd.FilterIndex = 1;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string jsonString = JsonSerializer.Serialize(dic);
                File.WriteAllText(sfd.FileName, jsonString, Encoding.GetEncoding("Shift_JIS"));
                MessageBox.Show("データを正常に保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 1:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 2:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 3:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 4:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 5:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        default:
                            MessageBox.Show("現在、VirtualSingerユニットは十分なデータがないため推測できません。", "未実装", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 1:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "319000000";
                            break;
                        case 1:
                            textBox1.Text = "117510000";
                            break;
                        case 2:
                            textBox1.Text = "88880000";
                            break;
                        case 3:
                            textBox1.Text = "38100000";
                            break;
                        case 4:
                            textBox1.Text = "23280000";
                            break;
                        case 5:
                            textBox1.Text = "17390000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 2:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "315000000";
                            break;
                        case 1:
                            textBox1.Text = "103850000";
                            break;
                        case 2:
                            textBox1.Text = "78040000";
                            break;
                        case 3:
                            textBox1.Text = "39000000";
                            break;
                        case 4:
                            textBox1.Text = "27320000";
                            break;
                        case 5:
                            textBox1.Text = "19410000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 3:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "350000000";
                            break;
                        case 1:
                            textBox1.Text = "150450000";
                            break;
                        case 2:
                            textBox1.Text = "101110000";
                            break;
                        case 3:
                            textBox1.Text = "54190000";
                            break;
                        case 4:
                            textBox1.Text = "35110000";
                            break;
                        case 5:
                            textBox1.Text = "24680000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 4:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "340000000";
                            break;
                        case 1:
                            textBox1.Text = "171540000";
                            break;
                        case 2:
                            textBox1.Text = "139450000";
                            break;
                        case 3:
                            textBox1.Text = "60780000";
                            break;
                        case 4:
                            textBox1.Text = "38620000";
                            break;
                        case 5:
                            textBox1.Text = "29710000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 5:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "382000000";
                            break;
                        case 1:
                            textBox1.Text = "172520000";
                            break;
                        case 2:
                            textBox1.Text = "145950000";
                            break;
                        case 3:
                            textBox1.Text = "57770000";
                            break;
                        case 4:
                            textBox1.Text = "42060000";
                            break;
                        case 5:
                            textBox1.Text = "36380000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                case 6:
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0:
                            textBox1.Text = "309390000";
                            break;
                        case 1:
                            textBox1.Text = "135170000";
                            break;
                        case 2:
                            textBox1.Text = "90610000";
                            break;
                        case 3:
                            textBox1.Text = "58680000";
                            break;
                        case 4:
                            textBox1.Text = "40880000";
                            break;
                        case 5:
                            textBox1.Text = "30840000";
                            break;
                        default:
                            MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;
                default:
                    MessageBox.Show("選択された情報が見つかりません。", "不正な値", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                textBox1.Enabled = false;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
            }
            else
            {
                textBox1.Enabled = true;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int bonus = int.Parse(textBox2.Text) + int.Parse(textBox6.Text) + (10 * (int.Parse(textBox3.Text) + int.Parse(textBox4.Text))) + (int.Parse(textBox5.Text) / 10);
            int ppt = (int.Parse(textBox8.Text) * magnificate(int.Parse(textBox7.Text)));
            int frequency = bonus / int.Parse(textBox7.Text);
            label26.Text = (frequency * ppt).ToString();
            label28.Text = ((int.Parse(textBox10.Text) * frequency * 24) / ((dateTimePicker2.Value - dateTimePicker1.Value).TotalHours * 60)).ToString("F");
            label32.Text = textBox1.Text;
            label35.Text = ((float)(float.Parse(label26.Text) / float.Parse(textBox1.Text) * 100)).ToString("F3");
        }

        private int magnificate(int consumption)
        {
            switch (consumption)
            {
                case 0:
                    return 1;
                case 1:
                    return 5;
                case 2:
                    return 10;
                case 3:
                    return 15;
                case 4:
                    return 19;
                case 5:
                    return 23;
                case 6:
                    return 26;
                case 7:
                    return 29;
                case 8:
                    return 31;
                case 9:
                    return 33;
                case 10:
                    return 35;
                default:
                    return 0;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, now.Day + 9, 21, 0, 0);
            dateTimePicker1.Value = start;
            dateTimePicker2.Value = end;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("キャンペーン等で付与されるライブボーナスアイテムです。\r\nほとんどが1度の使用で10回復ですが、それ以外のものは「その他」として計上してください。",
                "ヘルプ:限定ドリンク",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
