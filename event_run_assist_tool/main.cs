using Microsoft.Win32;
using System;

namespace event_run_assist_tool
{
    internal class main
    {
        static string temp_directory = "";
        public static string directory()
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
                            // キーに値を設定
                            key1.SetValue("path", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat");
                            temp_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                        }
                    }
                    // キーの値を読み込む
                    object value = key.GetValue("path");
                    if (value == null)
                    {
                        using (RegistryKey key1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\erat"))
                        {
                            key1.SetValue("path", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat");
                            temp_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\erat";
                        }
                    }
                    else
                    {
                        temp_directory = $"{value}";
                        return temp_directory;
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
