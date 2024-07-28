using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_run_assist_tool
{
    internal class main
    {
        string temp_directory = "";
        public string directory ()
        {
            if (temp_directory == "")
            {
                // レジストリキーを開く
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\erat"))
                {
                    if (key == null)
                    {
                        using (RegistryKey key1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\erat"))
                        {
                            if (key1 == null)
                            {
                                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                            }

                            // キーに値を設定
                            key1.SetValue("path", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat");
                        }
                        temp_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                    }
                    // キーの値を読み込む
                    object value = key.GetValue("path");
                    if (value == null)
                    {
                        using (RegistryKey key1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\erat"))
                        {
                            if (key1 == null)
                            {
                                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                            }

                            // キーに値を設定
                            key1.SetValue("path", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat");
                        }
                        temp_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                    }
                    else
                    {
                        return $"{value}";
                    }
                }
            }
            else
            {
                return temp_directory;
            }
        }
    }
}
