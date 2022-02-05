using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace TrayToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TrayIconClickCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("The New command was invoked");
            Hardcodet.Wpf.TaskbarNotification.Interop.Point p = myNotifyIcon.GetPopupTrayPosition();
            //this.Left = p.X;

            GetCursorPos(out var pt);
            //pt.X = 0;

            this.Left = pt.X - this.Width / 2;
            this.Top = p.Y - this.Height;

            this.Show();
        }
        #region dll imports
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        // DON'T use System.Drawing.Point, the order of the fields in System.Drawing.Point isn't guaranteed to stay the same.

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }

        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (string item in Directory.GetFiles(@"E:\Users\Anwender\Desktop\⠀"))
            foreach (string item in Directory.GetFiles(@"E:\Visual Studio Test\StylingOfStuff\bin\Debug\Test"))
            {
                string text = System.IO.Path.GetFileName(item);
                CreateButtonAndAddToStackPanel(item, text);
            }
            ///////////
            //////////
            /////////
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("test.xml");
            XmlNodeList? itemNodes = xmlDoc.SelectNodes("//buttons/button");
            int index = 0;
            foreach (XmlNode itemNode in itemNodes)
            {
                XmlNode textNode = itemNode.SelectSingleNode("text");
                XmlNode commandNode = itemNode.SelectSingleNode("command");
                if ((textNode != null) && (commandNode != null))
                {
                    //Button btn = new Button();
                    //btn.Content = textNode.InnerText;
                    //btn.Click += Btn_Click;
                    //btn.Tag = commandNode.InnerText;

                    CreateButtonAndAddToStackPanel(commandNode.InnerText, textNode.InnerText);

                    if (itemNode.Attributes["color"] != null)
                    {
                        //try
                        //{
                        //    btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(itemNode.Attributes["color"].Value));
                        //}
                        //catch (Exception) { }
                    }
                    if (itemNode.Attributes["background"] != null)
                    {
                        //try
                        //{
                        //    btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(itemNode.Attributes["background"].Value));
                        //}
                        //catch (Exception) { }
                    }

                    //stackPanel.Children.Add(btn);
                    index++;
                }
            }
        }


        private void CreateButtonAndAddToStackPanel(string filePath, string text)
        {
            ActionIconButton actionIconButton = new(text);
            actionIconButton.Icon = filePath;
            //actionIconButton.Text = item;
            actionIconButton.Command = filePath;
            //actionIconButton.BG = "Black";
            //actionIconButton.BG = "#2B2B2B";
            actionIconButton.LightMode = false;
            actionIconButton.Click += ActionIconButton_Click;
            stackPanel.Children.Add(actionIconButton);
        }

        private async void ActionIconButton_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //Process.Start(((ActionIconButton)sender).Command);
            //using (Process p = new Process())
            //{
            //    p.StartInfo = new ProcessStartInfo()
            //    {
            //        CreateNoWindow = true,
            //        UseShellExecute = true,
            //        Verb = "print",
            //        //FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\doc.pdf"
            //        FileName = "cmd.exe",
            //        //Arguments = ((ActionIconButton)sender).Command,
            //    };
            //    p.Start();
            //}
            //Cursor = Cursors.AppStarting;

            Mouse.OverrideCursor = Cursors.AppStarting;
            try
            {
                // do stuff
            }
            finally
            {
                //Mouse.OverrideCursor = null;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd";
            processStartInfo.Arguments = "/c \"" + ((ActionIconButton)sender).Command + "\"";
            //processStartInfo.UseShellExecute = true;
            //processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;
            Process.Start(processStartInfo);
            //Process.Start("cmd");
            //Cursor = Cursors.Arrow;
            await Task.Delay(1000);
            Mouse.OverrideCursor = null;
        }
    }
}