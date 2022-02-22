using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrayToolbar
{
    /// <summary>
    /// Interaction logic for SettingsWnd.xaml
    /// </summary>
    public partial class SettingsWnd : Window
    {
        public SettingsWnd()
        {
            InitializeComponent();
            //Properties.Settings.Default.Save();
            this.Loaded += SettingsWnd_Loaded;
        }

        private void SettingsWnd_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            tb_path.Text = Properties.Settings.Default.Path;
        }

        private void ChangePath_Click(object sender, RoutedEventArgs e)
        {
            /*
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                tb_path.Text = openFileDlg.FileName;
                Properties.Settings.Default.Path = openFileDlg.FileName;
            }
            */

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();


                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tb_path.Text = dialog.SelectedPath;
                    Properties.Settings.Default.Path = dialog.SelectedPath;
                }
            }
            tb_path.Text = Properties.Settings.Default.Path;
            Properties.Settings.Default.Save();
        }

        private void tb_path_LostFocus(object sender, RoutedEventArgs e)
        {
        //    Properties.Settings.Default.Path = ((TextBox)sender).Text;
        //    //SetAppSetting(Properties.Settings.Default.Path, ((TextBox)sender).Text);
        }

        private void tb_path_KeyDown(object sender, KeyEventArgs e)
        {
        //    Properties.Settings.Default.Path = ((TextBox)sender).Text;

        //    //SetAppSetting(Properties.Settings.Default., ((TextBox)sender).Text);
        //    //Properties.Settings.Default.Path = "";
        //    //Properties.Settings.Default.Save();
        //    //Properties.Settings.Default.Path = "E:\Users\Anwender\Desktop\⠀";
        }

        private void ShowOnlyFiles_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.showOnlyFiles = true;
            Properties.Settings.Default.Save();
        }

        private void ShowOnlyFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.showOnlyFiles = false;
            Properties.Settings.Default.Save();
        }

        //private void SetAppSetting(Properties.Settings path, string text)
        //{
        //    path = text;
        //    Properties.Settings.Default.Save();
        //}
    }
}
