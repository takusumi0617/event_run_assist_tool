using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.ShowDialog();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // レジストリキーを開く
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\erat"))
            {
                if (key == null)
                {
                    main.directory();

                }
                else
                {
                    // キーの値を読み込む
                    object value = key.GetValue("path");
                    if (value == null)
                    {
                        main.directory();
                    }
                }
            }
            initialization.initialize();
            
            var (warningTexts, area) = await warning.GetWarningsAsync();
            Console.WriteLine($"https://www.jma.go.jp/bosai/warning/#area_type=class20s&area_code={warning.CLASS_AREA_CODE}&lang=ja");
            Console.WriteLine($"{area}の気象警報・注意報");
            if (warningTexts.Count > 0)
            {
                warningTexts.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("現在発表警報・注意報はありません。");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "変数データを選択してください";
            ofd.InitialDirectory = $"{main.directory()}\\data";
            ofd.Filter = "変数ファイル(*.json)|*.json|すべてのファイル(*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Form form3 = new Form3(ofd.FileName);
                form3.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form license = new license();
            license.ShowDialog();
        }
    }
}
