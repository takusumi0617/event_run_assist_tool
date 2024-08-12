using Microsoft.Win32;
using System;
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

        private void Form1_Load(object sender, EventArgs e)
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
    }
}
