using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Path = System.IO.Path;

namespace TrayToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ActionIconButton> actionIconButtonsList;

        List<Process> activeProcesses;
        List<ActionIconButton> btnToHideIfProcStarted;


        //int itemHeight = 25;
        int itemHeight = 20;
        string xmlFile = @"E:\Visual Studio 2021\TrayToolbar\bin\test.xml";

        public MainWindow()
        {
            InitializeComponent();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            currentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            //this.Visibility = Visibility.Hidden;

            this.Left = -11000;
            this.Top = -11111;
            this.ShowInTaskbar = false;

            actionIconButtonsList = new List<ActionIconButton>();

            //activeProcessesAndBtns = new Dictionary<Process, ActionIconButton>();

            activeProcesses = new List<Process>();
            btnToHideIfProcStarted = new List<ActionIconButton>();

            ChangeWindowTheme();

            string[] args = Environment.GetCommandLineArgs();
            //Debug.WriteLine(args[0]);
            //MessageBox.Show(args[1]);

            for (int i = 0; i < args.Length; i++)
            {
                switch (i)
                {
                    case 1:
                        xmlFile = args[i];
                        break;
                    default:
                        break;
                }

                //_ = i switch
                //{
                //    0 => xmlFile = args[i]
                //};



            }

            //foreach (string arg in args) {

            //    switch (arg)
            //    {
            //        case 
            //        default:
            //            break;
            //    }

            //}

            Microsoft.Win32.SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            //File.Create("ddddd124.txt");

            //StreamWriter sw = File.AppendText("log1221.txt");
            //sw.WriteLine(DateTime.Now + " " + e);

            using (StreamWriter sw = File.AppendText("error.txt"))
            {
                sw.WriteLine(DateTime.Now + " " + e.Exception);
            }
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            //File.Create("ddddd124.txt");

            Exception e = (Exception)args.ExceptionObject;
            //Console.WriteLine("MyHandler caught : " + e.Message);
            //Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);

            //Debug.WriteLine("MyHandler caught : " + e.Message);

            //StreamWriter sw = File.AppendText("log.txt");
            //sw.WriteLine(DateTime.Now + " " + e.Message);

            using (StreamWriter sw = File.AppendText("error.txt"))
            {
                sw.WriteLine(DateTime.Now + " " + e.Message);
            }
        }


        private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            //if (e.Category == Microsoft.Win32.UserPreferenceCategory.General)
            //{
            //    Debug.WriteLine(SystemParameters.UxThemeName);
            //}

            ChangeWindowTheme();
        }

        private void ChangeWindowTheme()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", false);
            bool d = rk.GetValue("SystemUsesLightTheme").Equals(1);
            if (rk.GetValue("SystemUsesLightTheme").Equals(1))
            {
                ActionIconButton firstItem = new ActionIconButton("--", true);
                //foreach (var item in actionIconButtonsList)
                //{
                //    item.LightMode = true;

                //}
                for (int i = 0; i < actionIconButtonsList.Count; i++)
                {
                    actionIconButtonsList[i].LightMode = true;
                    if (i == 0)
                        firstItem = actionIconButtonsList[i];
                }
                //this.Background = firstItem.stackpanel.Background;
                mainBorder.Background = firstItem.stackpanel.Background;

                foreach (Button item in sp_WindowBar.Children)
                {
                    item.Foreground = Brushes.Black;
                    //item.Style = (Style)Resources["btnBlue"];
                    object resource = Application.Current.FindResource("TransparentStyle");
                    if (resource != null && resource.GetType() == typeof(Style))
                        item.Style = (Style)resource;

                    //object nul = Application.Current.FindResource("nul");

                    //item.FocusVisualStyle = (Style)nul;
                }
            }
            else
            {
                ActionIconButton firstItem = new ActionIconButton("--", false);
                for (int i = 0; i < actionIconButtonsList.Count; i++)
                {
                    actionIconButtonsList[i].LightMode = false;
                    if (i == 0)
                        firstItem = actionIconButtonsList[i];
                }
                mainBorder.Background = firstItem.stackpanel.Background;

                foreach (Button item in sp_WindowBar.Children)
                {
                    item.Foreground = Brushes.White;
                    //item.Style = (Style)Resources["btnBlue"];
                    //object resource = Application.Current.FindResource("btnBlue");
                    object resource = Application.Current.FindResource("TransparentStyle");
                    if (resource != null && resource.GetType() == typeof(Style))
                        item.Style = (Style)resource;
                }
            }
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
            double newX = pt.X - this.Width / 2;
            if (newX + this.Width >= 1920)
            {
                this.Left = 1920 - this.Width;
            }
            else
            {
                this.Left = pt.X - this.Width / 2;
            }

            this.Top = p.Y - this.Height;

            this.Show();

            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            this.Topmost = false;
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

            //*
            List<string> ignoreFiles = new List<string>();
            /////////
            XmlDocument xmlDoc1 = new XmlDocument();
            //xmlDoc1.Load("test.xml");
            xmlDoc1.Load(xmlFile);
            XmlNodeList? itemNodes1 = xmlDoc1.SelectNodes("//buttons/ignore");

            foreach (XmlNode itemNode in itemNodes1)
            {
                XmlNode textNode = itemNode.SelectSingleNode("text");
                if ((textNode != null))
                {
                    ignoreFiles.Add(textNode.InnerText);
                }
            }


            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(xmlFile);
            XmlNode? itemNode1 = xmlDoc2.SelectSingleNode("//buttons");

            if (itemNode1.Attributes != null)
            {
                if (itemNode1.Attributes["after"].Value.Equals("true"))
                {
                    CreatButtonsFromFiles(ignoreFiles);
                    CreateButtonsFromXML();
                }
                else
                {
                    CreateButtonsFromXML();
                    CreatButtonsFromFiles(ignoreFiles);
                }
            }
            else
            {
                CreateButtonsFromXML();
                CreatButtonsFromFiles(ignoreFiles);
            }

            //foreach (XmlNode itemNode in itemNodes1)
            //{
            //    XmlNode textNode = itemNode.SelectSingleNode("text");
            //    if ((textNode != null))
            //    {
            //        ignoreFiles.Add(textNode.InnerText);
            //    }
            //}




            ChangeWindowTheme();
            //*/
        }

        private void CreateButtonsFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNodeList? itemNodes = xmlDoc.SelectNodes("//buttons/button");

            int index = 0;
            foreach (XmlNode itemNode in itemNodes)
            {
                //string hideAttr = itemNode.Attributes["hideIfActive"].Value;
                string hideAttr = "";

                bool hideIfActive = false;
                if (itemNode.Attributes["hideIfActive"] != null)
                {
                    hideAttr = itemNode.Attributes["hideIfActive"].Value;
                    if (hideAttr.ToLower().Equals("true"))
                    {
                        hideIfActive = true;
                    }
                    else if (hideAttr.ToLower().Equals("´false"))
                    {
                        hideIfActive = false;
                    }
                }

                XmlNode textNode = itemNode.SelectSingleNode("text");
                XmlNode commandNode = itemNode.SelectSingleNode("command");
                XmlNode iconNode = itemNode.SelectSingleNode("icon");
                XmlNode fileNameNode = itemNode.SelectSingleNode("fileName");
                if ((textNode != null) && (commandNode != null))
                {
                    //Button btn = new Button();
                    //btn.Content = textNode.InnerText;
                    //btn.Click += Btn_Click;
                    //btn.Tag = commandNode.InnerText;

                    string icon = "";
                    if (iconNode != null)
                    {
                        icon = iconNode.InnerText;
                    }

                    string fileName = "cmd";
                    if (fileNameNode != null)
                    {
                        fileName = fileNameNode.InnerText;
                    }

                    CreateButtonAndAddToStackPanel(commandNode.InnerText, textNode.InnerText, icon, fileName, hideIfActive, xml: true);

                    //if (iconNode != null)
                    //{
                    //    CreateButtonAndAddToStackPanel(commandNode.InnerText, textNode.InnerText, iconNode.InnerText);
                    //}
                    //else
                    //{
                    //    CreateButtonAndAddToStackPanel(commandNode.InnerText, textNode.InnerText);
                    //}


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


                    //if (iconNode != null)
                    //{

                    //}

                    //stackPanel.Children.Add(btn);
                    index++;
                }
            }
        }

        private void CreatButtonsFromFiles(List<string> ignoreFiles)
        {
            //DirectoryInfo directory = new DirectoryInfo(@"E:\Users\Anwender\Desktop\⠀");
            //FileInfo[] files = directory.GetFiles();

            ////var sorted = files.OrderBy(f => );
            ////var orderedNodes = parent.Elements().OrderBy(f => f.Name.LocalName, new NodeComparer());
            //var filtered = files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            var dir = @"E:\Users\Anwender\Desktop\⠀\";
            List<string> files = Directory.GetFiles(@"E:\Users\Anwender\Desktop\⠀").ToList();
            //files.OrderBy(f => f).ToList();
            //files = files.OrderBy(f => Regex.Replace(f, @"~[\d-]", string.Empty)).ToList();

            int count = 0;
            foreach (string item in files)
            //foreach (string item in Directory.GetFiles(@"E:\Visual Studio Test\StylingOfStuff\bin\Debug\Test"))
            {
                //if (File.)
                //{
                //}

                //if (item.Contains("shutdown"))
                //{
                //    ;
                //}
                var item1 = item;
                if (item.StartsWith("~"))
                {
                    item1 = item.Remove(0, 2);
                }
                else
                {
                    File.Move(dir + item, dir + "~" + count + Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item));
                    count++;
                }

                if (ignoreFiles.Contains(System.IO.Path.GetFileName(item1)))
                    continue;

                string text = System.IO.Path.GetFileName(item1);
                CreateButtonAndAddToStackPanel(item1, text);
            }
        }

        private void CreateButtonAndAddToStackPanel(string filePath, string text, string icon = "", string fileName = "cmd", bool hideIFActive = false, bool xml = false)
        {
            ActionIconButton actionIconButton = new(text, false);
            //actionIconButton.Height = itemHeight;
            actionIconButton.SetHeight(itemHeight);

            if (icon == "")
            {
                //actionIconButton.Icon = filePath;
                actionIconButton.SetIconFromFile(filePath);
            }
            else
            {
                actionIconButton.SetIconFromFile(icon);

                //actionIconButton.Icon = icon;
            }

            actionIconButton.HideIfActive = hideIFActive;

            actionIconButton.FileName = fileName;


            //actionIconButton.Text = item;
            actionIconButton.Command = filePath;
            //actionIconButton.BG = "Black";
            //actionIconButton.BG = "#2B2B2B";
            //actionIconButton.LightMode = false;
            actionIconButton.Click += ActionIconButton_Click;
            stackPanel.Children.Add(actionIconButton);
            //this.Height = 18 + 33 * stackPanel.Children.Count;
            this.Height = 18 + itemHeight * stackPanel.Children.Count + 4;
            this.Width = 250;
            //this.Background = actionIconButton.stackpanel.Background;

            actionIconButtonsList.Add(actionIconButton);


            actionIconButton.FromXML = xml;
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

            var btn = ((ActionIconButton)sender);
            var cmd = btn.Command;
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            //processStartInfo.FileName = "cmd";

            processStartInfo.FileName = btn.FileName;

            //processStartInfo.Arguments = "/c \"" + ((ActionIconButton)sender).Command + "\"";
            //processStartInfo.Arguments = ((ActionIconButton)sender).Command;

            if (cmd.Contains("&"))
            {
                processStartInfo.Arguments = "/c " + cmd;
            }
            else
            {
                //processStartInfo.Arguments = "/c \"" + cmd + "\"";
                if (processStartInfo.FileName == "")
                    processStartInfo.FileName = cmd;
                else processStartInfo.Arguments = "/c \"" + cmd + "\"";
            }


            //processStartInfo.UseShellExecute = true;
            //processStartInfo.RedirectStandardOutput = true;
            if (cmd.ToLower().Contains("pause") || cmd.ToUpper().Contains("!SHOWWND"))
            {
                processStartInfo.CreateNoWindow = false;
                cmd?.Replace("!SHOWWND", "");
            }
            else
            {
                processStartInfo.CreateNoWindow = true;
            }

            //processStartInfo.CreateNoWindow = false;
            Process p = new Process();

            p.StartInfo = processStartInfo;


            //Process.Start("cmd");
            //Cursor = Cursors.Arrow;

            await Task.Delay(1000);

            p.Start();

            if (p.Responding && !p.HasExited)
            {
                if (btn.HideIfActive == true)
                {
                    btn.Visibility = Visibility.Collapsed;

                    activeProcesses.Add(p);
                    btnToHideIfProcStarted.Add(btn);

                    p.EnableRaisingEvents = true;
                    p.Exited += P_Exited;
                    p.Disposed += P_Disposed;
                }
            }

            

            //string myString = processesAndBtns[1][myTime] // Day 1 at that specific time.


            //await Task.Delay(500);
            Mouse.OverrideCursor = null;


            //p.Close();
        }

        private void P_Disposed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void P_Exited(object? sender, EventArgs e)
        {
            Process p = (Process)sender;
            for (int i = 0; i < activeProcesses.Count; i++)
            {
                //Process process = (Process)actionIconButtonsList[i];
                //Dictionary<Process, ActionIconButton>() d = new 

                if (p == activeProcesses[i])
                {
                    this.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate ()
                        {
                            btnToHideIfProcStarted[i].Visibility = Visibility.Visible;
                        }
                    ));


                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //if (!System.IO.File.Exists("test.xml"))
            //{
            //    //return false;
            //}
            //Clean up file path so it can be navigated OK
            //filePath = System.IO.Path.GetFullPath(filePath);

            //System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", "test.xml"));

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "OpenWith.exe";
            //startInfo.Arguments = "\"" + System.IO.Path.Combine(Environment.CurrentDirectory, "test.xml") + "\"";
            startInfo.Arguments = "\"" + xmlFile + "\"";
            //startInfo.WorkingDirectory =;

            //Process.Start("OpenWith.exe", "test.xml");
            Process.Start(startInfo);

            //return true;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }





        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        //private void stackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        private void stackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this.stackPanel)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this.stackPanel);
            }
        }

        private void stackPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            _realDragSource.ReleaseMouseCapture();
        }

        private void stackPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.stackPanel).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.stackPanel).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }

        private void stackPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void stackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement droptarget = e.Source as UIElement;
                int droptargetIndex = -1, i = 0;
                foreach (UIElement element in this.stackPanel.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {
                    this.stackPanel.Children.Remove(_realDragSource);
                    this.stackPanel.Children.Insert(droptargetIndex, _realDragSource);
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();

                SaveNewOrder(droptarget as ActionIconButton);
            }
        }

        private void SaveNewOrder(ActionIconButton btn)
        {
            if (btn.FromXML)
            {
                // load xml into document
                var s = System.Xml.Linq.XDocument.Load(xmlFile);

                // get the parent node
                var parent = s.Elements().FirstOrDefault();

                //// reorder nodes
                //var orderedNodes = parent.Elements().OrderBy(f => f.Name.LocalName, new NodeComparer());

                //// replace nodes
                //parent.ReplaceNodes(orderedNodes);

                // reorder nodes
                var orderedNodes = parent.Elements().OrderBy(f => f.Name.LocalName, new NodeComparer());

                // replace nodes
                parent.ReplaceNodes(orderedNodes);

                // save back to localtion
                s.Save(xmlFile);
            }
            else
            {
                //btn.FileName
                //File.copy
                //File.Move(btn.FileName, )
            }


        }

        void OrderButtons()
        {

        }
    }
}