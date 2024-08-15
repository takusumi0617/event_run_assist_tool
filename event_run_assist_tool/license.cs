using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace event_run_assist_tool
{
    public partial class license : Form
    {
        public license()
        {
            InitializeComponent();
        }

        private void license_Load(object sender, EventArgs e)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string licenseDirectory = Path.Combine(exeDirectory, "License");

            if (Directory.Exists(licenseDirectory))
            {
                var licenseFiles = Directory.GetFiles(licenseDirectory, "*.license");

                foreach (var file in licenseFiles)
                {
                    listBox1.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                MessageBox.Show("License directory not found.");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedFile = listBox1.SelectedItem.ToString();
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exeDirectory, "License", selectedFile + ".license");

                if (File.Exists(filePath))
                {
                    string fileContent = File.ReadAllText(filePath);
                    textBox1.Text = fileContent;
                }
            }
        }
    }
}
