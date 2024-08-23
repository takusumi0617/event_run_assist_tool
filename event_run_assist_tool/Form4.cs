using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            textBox1.Text = main.directory();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form5 = new Form5();
            form5.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != main.directory())
            {
                using (RegistryKey key1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\erat"))
                {
                    key1.SetValue("path", textBox1.Text);
                }
                MessageBox.Show("設定を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }
    }
}
