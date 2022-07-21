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

using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace TrayToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double dpiScaleX { get; set; } = 1;
        public double dpiScaleY { get; set; } = 1;

        public string settingsFilePath { get; set; } = "settings.xml";

        List<ActionIconButton> actionIconButtonsList;

        List<Process> activeProcesses;
        List<ActionIconButton> btnToHideIfProcStarted;


        //int itemHeight = 25;
        int itemHeight = 20;
        string xmlFile = "";
        string dir = @".";
        string dir2 = @"";

        //SolidColorBrush mainColor; //background
        //SolidColorBrush secondColor; //foreground

        public enum WindowThemes
        {
            none,
            system,
            app,
            light,
            dark,
            l, d
        }

        //WindowThemes theme = WindowThemes.dark;
        WindowThemes theme;
        public WindowThemes Theme
        {
            get
            {
                if (theme == WindowThemes.l)
                    Theme = WindowThemes.light;

                if (theme == WindowThemes.d)
                    Theme = WindowThemes.dark;

                if (theme == WindowThemes.dark || theme == WindowThemes.light)
                    return theme;

                bool light = false;

                if (theme == WindowThemes.system)
                {
                    RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", false);
                    light = rk.GetValue("SystemUsesLightTheme").Equals(1);
                }
                else if (theme == WindowThemes.app)
                {
                    RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", false);
                    light = rk.GetValue("AppsUseLightTheme").Equals(1);
                }

                if (light)
                    return WindowThemes.light;
                else
                    return WindowThemes.dark;
                //else if (theme == WindowThemes.app)
            }
            set
            {
                theme = value;
            }
        }

        public void LoadSettingsAndCreateButtons()
        {


            //WindowTheme = WindowTheme.


            LoadSettings();

            CreateButtons();

            List<string> ignoreFiles = new List<string>();
            /////////
            XmlDocument xmlDoc1 = new XmlDocument();
            //xmlDoc1.Load("test.xml");
            xmlDoc1.Load(xmlFile);
            XmlNodeList? itemNodes1 = xmlDoc1.SelectNodes("//buttons/ignore/text");

            foreach (XmlNode textNode in itemNodes1)
            {
                //XmlNode textNode = itemNode.SelectSingleNode("text");
                if ((textNode != null))
                {
                    ignoreFiles.Add(textNode.InnerText);
                }
            }


            //Color c1 = ((Color)ColorConverter.ConvertFromString("#EEEEEE"));
            //System.Drawing.Color c2 = System.Drawing.Color.FromArgb(c1.A, (int)(c1.R * 0.8), (int)(c1.G * 0.8), (int)(c1.B * 0.8));


            //string light = XmlSettings.Instance.GetValueOfSetting("lightColor") ?? "#EEEEEE";
            //string dark = XmlSettings.Instance.GetValueOfSetting("darkColor") ?? "#2B2B2B";


            string lightBg = XmlSettings.Instance.GetValueOfSetting("lightBg") ?? "#EEEEEE";
            string lightFg = XmlSettings.Instance.GetValueOfSetting("lightFg") ?? "#000000";
            string darkBg = XmlSettings.Instance.GetValueOfSetting("darkBg") ?? "#2B2B2B";
            string darkFg = XmlSettings.Instance.GetValueOfSetting("darkFg") ?? "#FFFFFF";

            //SolidColorBrush white = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#EEEEEE")));
            //SolidColorBrush black = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#EEEEEE")));

            //SolidColorBrush white = new SolidColorBrush(Colors.White);
            //SolidColorBrush black = new SolidColorBrush(Colors.Black);

            if (Theme == WindowThemes.light)
            {
                //ChangeWindowColors(white, black, true);
                //mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
                mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lightBg));

                //mainBorder.Background = all_sp.Background = new SolidColorBrush(Colors.Black);
                ChangeForegroundOfWindowBarButtons(lightFg, ChangeLightnessOfColor(lightBg, 1.25));
            }
            else if (Theme == WindowThemes.dark)
            {
                //ChangeWindowColors(black, white, false);
                //mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
                mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(darkBg));

                ChangeForegroundOfWindowBarButtons(darkFg, ChangeLightnessOfColor(darkBg, 1.25));
            }

            Order(ignoreFiles);

            //ChangeWindowTheme();


            if (Theme == WindowThemes.light)
            {
                ChangeWindowColors(lightBg, lightFg, true);
                //mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
            }

            else if (Theme == WindowThemes.dark)
            {
                ChangeWindowColors(darkBg, darkFg, false);
                //mainBorder.Background = all_sp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
            }


        }

        private void ChangeForegroundOfWindowBarButtons(string color, SolidColorBrush hover)
        {
            foreach (Button btn in sp_WindowBar.Children)
            {
                //btn.Tag = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
                //btn.Tag = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                //btn.Tag = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hover));
                btn.Tag = hover;
            }
        }

        private MColor MakeColorLighter(MColor mColor, double val)
        {
            //DColor lighterColor = System.Drawing.Color.FromArgb(mColor.A, (int)(mColor.R * val), (int)(mColor.G * val), (int)(mColor.B * val));
            //        DColor lighterColor = System.Drawing.Color.FromArgb(mColor.A,
            //Math.Clamp((int)(mColor.R * val), 0, 255),
            //Math.Clamp((int)(mColor.G * val), 0, 255),
            //Math.Clamp((int)(mColor.B * val), 0, 255));
            DColor lighterColor = System.Drawing.Color.FromArgb(mColor.A, (int)(mColor.R * val).Clamp(0, 255), (int)(mColor.G * val).Clamp(0, 255), (int)(mColor.B * val).Clamp(0, 255));
            return System.Windows.Media.Color.FromArgb(lighterColor.A, lighterColor.R, lighterColor.G, lighterColor.B);
        }

        private SolidColorBrush ChangeLightnessOfColor(string col, double val)
        {
            MColor mColor = (Color)ColorConverter.ConvertFromString((string)col);
            DColor lighterColor = System.Drawing.Color.FromArgb(mColor.A, (int)(mColor.R * val).Clamp(0, 255), (int)(mColor.G * val).Clamp(0, 255), (int)(mColor.B * val).Clamp(0, 255));
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(lighterColor.A, lighterColor.R, lighterColor.G, lighterColor.B));
        }

        public System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void LoadSettings()
        {
            XmlSettings instance = XmlSettings.Instance;
            instance.Load("settings.xml");

            //
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(xmlFile);
            XmlNode? itemNode1 = xmlDoc2.SelectSingleNode("//buttons");


            foreach (XmlAttribute attr in itemNode1.Attributes)
            {
                if (attr.Name == "path")
                {
                    if (Directory.Exists(attr.InnerXml))
                    {
                        dir = attr.InnerXml;
                    }
                }
            }
            //

            // Settings File
            dir = instance.GetValueOfSetting("path") ?? dir;
            dir2 = instance.GetValueOfSetting("path2") ?? dir2;

            //Theme = instance.GetValueOfSetting("theme") ?? WindowThemes.system;
            bool b = Enum.TryParse<WindowThemes>(instance.GetValueOfSetting("theme"), true, out WindowThemes result);

            //if (result == WindowThemes.none)
            //{
            //    b = Enum.TryParse<WindowThemes>(instance.GetValueOfSetting("theme"), true, out WindowThemes result1);
            //}

            Theme = result;
            //if (!result)
            //{
            //    ;
            //}


        }

        public void ResetPosition()
        {
            Hardcodet.Wpf.TaskbarNotification.Interop.Point p = myNotifyIcon.GetPopupTrayPosition();
            Left = p.X - Width - 5;
            Top = p.Y - Height - 5;
            this.Activate();
        }

        public MainWindow()
        {
            InitializeComponent();

            ContextMenu contextMenu = new();
            MenuItem menuItem = new();
            menuItem.Click += (sender, args) => ResetPosition();
            menuItem.Header = "Reset Position";
            contextMenu.Items.Add(menuItem);

            contextMenu.AddToCm("Exit", () => App.Current.Shutdown());

            //contextMenu.Items.Add(new MenuItem() {Header = "Reset Position"}.Click += (sender, args) => ResetPosition());

            myNotifyIcon.ContextMenu = contextMenu;

            Microsoft.Win32.SystemEvents.PowerModeChanged += (sender, args) =>
            {
                LoadSettingsAndCreateButtons();
            };

            DpiChanged += MainWindow_DpiChanged;

            /*
            List<Setting> settings = XmlSettings.Load("settings.xml");

            //foreach (Setting setting in settings)

            //if (settings.Contains())
            if (settings.Any(s => s.Name.Equals("test")))
            //if (settings.Contains("test"))
            {
                ;
                //MessageBox.Show(settings["test"].Value);
                Setting setting = settings.Find(s => s.Name.Equals("test"));
                MessageBox.Show(setting.Value);
            }
            */
            //var t = XmlSettings.Instance;

            /*
            XmlSettings instance = XmlSettings.Instance;
            instance.Load("settings.xml");
            var test = XmlSettings.Instance.settings.Find(s => s.Name.Equals("test")).Value;
            //var test1 = instance.settings.Find(s => s.Name.Equals("path")).Value;

            //dir ??= XmlSettings.Instance.settings.Find(s => s.Name.Equals("path")).Value;
            //dir = instance.settings.Find(s => s.Name.Equals("path")).Value;

            dir = instance.GetValueOfSetting("path") ?? dir;
            */
            //return;

            /*
            if (!File.Exists(xmlFile))
            {
                //File.OpenWrite("default.xml");
                using (FileStream sr = File.OpenWrite("default.xml"))
                {

                }

                xmlFile = "default.xml";
            }*/


            //GetXmlFile(Path.Combine(Directory.GetCurrentDirectory(), "items.xml"), 0);
            //MessageBox.Show("1.", Directory.GetCurrentDirectory());

            //bool foundXmlFile = GetXmlFile(Directory.GetCurrentDirectory(), 0);
            xmlFile = GetXmlFile(Directory.GetCurrentDirectory(), 0);
            if (xmlFile == "")
            {
                using (StreamWriter sw = File.CreateText("items.xml"))
                {
                    //sw.WriteLine(@"<buttons after='false' path='.'>");
                    //sw.Write(@"<button>\n        <text>Example Item (opens cmd)</text>\n        <icon>C:\WINDOWS\system32\cmd.exe</icon>\");
                    sw.Write(@"<buttons after='false' path='.'>
    <button>
        <text>Example Item (opens cmd)</text>
        <command>start cmd</command>
        <!-- <command>cmd !SHOWWND</command> -->
        <icon>C:\WINDOWS\system32\cmd.exe</icon>
        <fileName>cmd</fileName>
    </button>
</buttons>");
                    //sw.WriteLine(@"</buttons>");
                }

                xmlFile = "items.xml";
            }
            /*
            Action<string, int> settingsFunc = (string s, int count) =>
            {
                if (File.Exists(Path.Combine(s, "items.xml")))
                {
                    xmlFile = Path.Combine(s, "items.xml");
                    return true;
                }
                else
                {
                    var currentPath = Path.GetDirectoryName(s);

                    if (currentPath == null)
                    {
                        return false;
                    }


                    foreach (var fileName in Directory.GetFiles(currentPath))
                    {
                        if (fileName.Equals("items.xml"))
                        {
                            xmlFile = s;
                            return true;
                        }

                    }

                    var i = s.LastIndexOf(@"\");
                    s = s.Remove(i, s.Count() - i);

                    GetXmlFile(s, ++count);

                    return false;
            };*/

            settingsFilePath = GetXmlFile(Directory.GetCurrentDirectory(), 0, "settings.xml");
            if (string.IsNullOrWhiteSpace(settingsFilePath))
            {
                using (StreamWriter sw = File.CreateText("settings.xml"))
                {
                    sw.Write("<settings></settings>");
                }

                settingsFilePath = "items.xml";
            }



            var exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //MessageBox.Show("2.");

            //if(exeDir != null)
            //GetXmlFile(Path.Combine(exeDir, "items.xml"), 0);
            //GetXmlFile(Path.Combine(exeDir), 0);


            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            currentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            //this.Visibility = Visibility.Hidden;

            this.Left = -11000;
            this.Top = -11111;
            this.ShowInTaskbar = false;

            actionIconButtonsList = new List<ActionIconButton>();

            //activeProcessesAndBtns = new Dictionary<Process, ActionIconButton>();

            activeProcesses = new List<Process>();
            btnToHideIfProcStarted = new List<ActionIconButton>();

            //ChangeWindowTheme();

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

        private void MainWindow_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            //throw new NotImplementedException();
            dpiScaleX = e.NewDpi.DpiScaleX;
            dpiScaleY = e.NewDpi.DpiScaleY;
        }

        private string GetXmlFile(string s, int count, string name = "items.xml")
        {
            //MessageBox.Show(s.ToString() + " --- " + count);
            //var file = "items.xml";
            if (File.Exists(Path.Combine(s, name)))
            {
                //xmlFile = s;
                xmlFile = Path.Combine(s, "items.xml");
                return Path.Combine(s, "items.xml");
                //MessageBox.Show(xmlFile, "found1");
                //MessageBox.Show(s, "found2");
            }
            else
            {
                //foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory()))
                var currentPath = Path.GetDirectoryName(s);

                if (currentPath == null)
                {
                    return "";
                }


                foreach (var fileName in Directory.GetFiles(currentPath))
                {
                    if (fileName.Equals(name))
                    //if (Path.GetFileName(fileName).Equals("items.xml"))
                    {
                        xmlFile = s;
                        return s;
                    }
                    //count++;
                    //GetXmlFile(System.IO.Path.GetFileName(fileName), count);
                    //GetXmlFile(Path.GetDirectoryName(s), count++);
                    //GetXmlFile(s + @"\..\", count++);
                }

                var i = s.LastIndexOf(@"\");
                s = s.Remove(i, s.Count() - i);
                //GetXmlFile(Directory.GetParent(Path.GetDirectoryName(s)).FullName, ++count);

                GetXmlFile(s, ++count);
                try
                {
                    //if (Directory.GetParent(Path.GetDirectoryName(s)) != null)
                    //{
                    //    GetXmlFile(Directory.GetParent(Path.GetDirectoryName(s)).FullName, ++count);
                    //}

                }
                catch (Exception)
                {

                    //throw;
                }
                return "";
            }

            if (count >= 4)
            {
                /*
                using (FileStream sr = File.OpenWrite("default.xml"))
                {
                    sr.Write()
                }*/

                return "";
            }
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

        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
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

            //ChangeWindowTheme();
        }

        //void ChangeWindowColors(SolidColorBrush main, SolidColorBrush second, bool light)
        void ChangeWindowColors(string bg, string fg, bool light)
        {



            //MColor lightCol = ((Color)ColorConverter.ConvertFromString(light));
            //MColor darkCol = ((Color)ColorConverter.ConvertFromString(dark));
            ////DColor lightColor = System.Drawing.Color.FromArgb(lightCol.A, (int)(lightCol.R * 0.8), (int)(lightCol.G * 0.8), (int)(lightCol.B * 0.8));
            ////DColor lightColor = System.Drawing.Color.FromArgb(lightCol.A, lightCol.R, lightCol.G, lightCol.B);

            ////MakeColorLighter(lightCol, 1.25);

            ////light = lightColor.

            //SolidColorBrush lightHover = new SolidColorBrush(MakeColorLighter(lightCol, 1.25));
            ////SolidColorBrush white = new SolidColorBrush(ToMediaColor(lightColor));
            //SolidColorBrush darkHover = new SolidColorBrush(MakeColorLighter(darkCol, 1.25));


            MColor mColor = ((Color)ColorConverter.ConvertFromString(bg));
            SolidColorBrush hover = new SolidColorBrush(MakeColorLighter(mColor, 1.25));

            SolidColorBrush bgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bg));
            SolidColorBrush fgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg));

            ActionIconButton btn = null;
            for (int i = 0; i < actionIconButtonsList.Count; i++)
            {
                //actionIconButtonsList[i].UpdateColors
                //    (
                //    (
                //    (Color)ColorConverter.ConvertFromString(bg),
                //    (
                //    (Color)ColorConverter.ConvertFromString(fg)
                //    );

                //actionIconButtonsList[i].LightMode = light;
                //todo

                actionIconButtonsList[i].UpdateColors(bgColor, fgColor);
                actionIconButtonsList[i].UpdateHoverColor(hover);
                //if (light)
                //{
                //    actionIconButtonsList[i].LightMode = light;
                //}
                //if (i == 0)
                //    btn = actionIconButtonsList[i];
            }
            /*
            //mainBorder.Background = all_sp.Background = this.Background = main;

            if (light)
                mainBorder.Background = all_sp.Background = this.Background =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(btn.lightColorString));
            else
                mainBorder.Background = all_sp.Background = this.Background =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(btn.darkColorString));
            */
        }

        void ChangeWindowTheme()
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.White);
            //mainBorder.Background =
            //all_sp.Background = brush;

            //ActionIconButton firstItem = new ActionIconButton("--", true);
            ActionIconButton firstItem = null;

            for (int i = 0; i < actionIconButtonsList.Count; i++)
            {
                actionIconButtonsList[i].LightMode = true;
                if (i == 0)
                    firstItem = actionIconButtonsList[i];
            }

            mainBorder.Background = brush;

            /*
            foreach (Button item in sp_WindowBar.Children)
            {
                item.Foreground = Brushes.Black;
                //item.Style = (Style)Resources["btnBlue"];
                object resource = Application.Current.FindResource("TransparentStyle");
                if (resource != null && resource.GetType() == typeof(Style))
                    item.Style = (Style)resource;
            }*/
        }

        private void ChangeWindowThemeOld()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", false);
            //bool d = rk.GetValue("SystemUsesLightTheme").Equals(1);
            bool d = true;
            //if (rk.GetValue("SystemUsesLightTheme").Equals(1))
            if (d)
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



                //foreach (var item in all_sp.Children)
                //{
                //    if (item is StackPanel)
                //    {
                //        StackPanel stackPanel = (StackPanel)item;
                //        stackPanel.Background = Brushes.Black;
                //    }
                //}

            }
            else
            {
                //foreach (var item in all_sp.Children)
                //{
                //    if (item is StackPanel)
                //    {
                //        StackPanel stackPanel = (StackPanel)item;
                //        stackPanel.Background = Brushes.White;
                //    }
                //}

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
            this.Show();
            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            this.Topmost = false;
            this.Activate();


            //Left = p.X-Width-5;
            //Top = p.Y - Height - 5;
            return;
            //this.Left = p.X;
            //x = 1606
            //y = 1017

            //x = 2560
            //y = 1550
            //Left = p.X * 1.5;
            //Top = p.Y * 1.5;

            //return;
            //SetProcessDPIAware();

            GetCursorPos(out var pt);
            //pt.X = 0;

            double dpiWidth = Width * dpiScaleX;
            double dpiHeight = Height * dpiScaleY;


            //double newX = pt.X*dpiScaleX - dpiWidth / 2;
            double newX = pt.X * dpiScaleX - (Width / 2) * dpiScaleX;
            //newX *= dpiScaleX;
            Left = newX;
            //Left = 10;
            /*
            if (newX + dpiWidth >= 1920)
            {
                this.Left = 1920 - dpiWidth;
            }
            else
            {
                this.Left = pt.X - dpiWidth / 2;
            }*/

            //this.Top = p.Y*dpiScaleY - dpiHeight;
            this.Top = 0;
            this.Show();

            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            this.Topmost = false;

            this.Activate();
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
            //CreateButtons();
            LoadSettingsAndCreateButtons();
            ResetPosition();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                //CreateButtons();
                LoadSettingsAndCreateButtons();
            }
        }


        public void CreateButtons()
        {

            stackPanel.Children.Clear();
            xml_sp.Children.Clear();
            //stackPanel.Children.Add(new System.Windows.Controls.TextBlock());

            //*

            //moved to LoadSettings();



            //CreatButtonsFromFiles(ignoreFiles);

            //Order(ignoreFiles);

            //CreatButtonsFromFiles(ignoreFiles);
            //CreateButtonsFromXML();


            //foreach (XmlNode itemNode in itemNodes1)
            //{
            //    XmlNode textNode = itemNode.SelectSingleNode("text");
            //    if ((textNode != null))
            //    {
            //        ignoreFiles.Add(textNode.InnerText);
            //    }
            //}




            //ChangeWindowTheme();
            //*/
        }

        private void Order(List<string> ignoreFiles)
        {
            bool xmlBelowFiles = true;

            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(xmlFile);
            XmlNode? itemNode1 = xmlDoc2.SelectSingleNode("//buttons");

            //if (itemNode1.Attributes != null)
            if (itemNode1.Attributes["after"] != null)
            {
                if (itemNode1.Attributes["after"].Value.Equals("true"))
                {
                    //CreatButtonsFromFiles(ignoreFiles);
                    //CreateButtonsFromXML();
                    //all_sp.Children.Remove(sep);
                    //all_sp.Children.Add(sep);
                    //all_sp.Children.Remove(xml_sp);
                    //all_sp.Children.Add(xml_sp);
                    //CreatButtonsFromFiles(ignoreFiles);
                    //CreateButtonsFromXML();
                    //SetXmlBelowFiles(ignoreFiles);

                    //xmlBelowFiles = false;
                    xmlBelowFiles = true;
                }
                else
                {
                    ////CreateButtonsFromXML();
                    ////CreatButtonsFromFiles(ignoreFiles);
                    //all_sp.Children.Remove(sep);
                    //all_sp.Children.Add(sep);
                    //all_sp.Children.Remove(stackPanel);
                    //all_sp.Children.Add(stackPanel);
                    //CreateButtonsFromXML();
                    //CreatButtonsFromFiles(ignoreFiles);
                    //SetXmlAboveFiles(ignoreFiles);

                    //xmlBelowFiles &= true;

                    xmlBelowFiles = false;
                }
            }
            else
            {
                //all_sp.Children.Remove(sep);
                //all_sp.Children.Add(sep);
                //all_sp.Children.Remove(stackPanel);
                //all_sp.Children.Add(stackPanel);
                //CreateButtonsFromXML();
                //CreatButtonsFromFiles(ignoreFiles);
                //SetXmlAboveFiles(ignoreFiles);

                //xmlBelowFiles |= true;
                xmlBelowFiles = false;
            }

            XmlSettings instance = XmlSettings.Instance;
            //instance.Load("settings.xml");
            instance.Load(settingsFilePath);
            //xmlBelowFiles = bool.Parse(instance.GetValueOfSetting("xmlAboveFiles"));

            //string? xmlBelowFilesValue = instance.GetValueOfSetting("xmlBelowFiles");
            //if(xmlBelowFilesValue != null)
            //    xmlBelowFiles = bool.Parse(xmlBelowFilesValue);

            string? xmlBelowFilesValue = instance.GetValueOfSetting("xmlBelowFiles");
            if (xmlBelowFilesValue != null)
            {
                //xmlBelowFiles = bool.TryParse(xmlBelowFilesValue, out bool result) && result == true;
                try
                {
                    xmlBelowFiles = bool.Parse(xmlBelowFilesValue);
                }
                catch (Exception)
                {
                }
            }



            //if (xmlAboveFiles != null)
            //{
            //    if (xmlAboveFiles == "true")
            //        SetXmlBelowFiles(ignoreFiles);
            //    else if (xmlAboveFiles == "false")
            //        SetXmlBelowFiles(ignoreFiles);
            //}

            if (xmlBelowFiles)
                SetXmlBelowFiles(ignoreFiles);
            else
                SetXmlAboveFiles(ignoreFiles);
        }

        void SetXmlBelowFiles(List<string> ignoreFiles)
        {
            all_sp.Children.Remove(sep);
            all_sp.Children.Add(sep);
            all_sp.Children.Remove(xml_sp);
            all_sp.Children.Add(xml_sp);
            CreatButtonsFromFiles(ignoreFiles);
            CreatButtonsFromFiles2(ignoreFiles, dir2);
            CreateButtonsFromXML();
        }

        void SetXmlAboveFiles(List<string> ignoreFiles)
        {
            all_sp.Children.Remove(sep);
            all_sp.Children.Add(sep);
            all_sp.Children.Remove(stackPanel);
            all_sp.Children.Add(stackPanel);
            CreateButtonsFromXML();
            CreatButtonsFromFiles(ignoreFiles);
            CreatButtonsFromFiles2(ignoreFiles, dir2);
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

        //public void CreateButtonsFromFilesAndFolders()
        //{

        //}

        private void CreatButtonsFromFiles(List<string> ignoreFiles)
        {
            #region
            /*
            //DirectoryInfo directory = new DirectoryInfo(@"E:\Users\Anwender\Desktop\⠀");
            //FileInfo[] files = directory.GetFiles();

            ////var sorted = files.OrderBy(f => );
            ////var orderedNodes = parent.Elements().OrderBy(f => f.Name.LocalName, new NodeComparer());
            //var filtered = files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));

            //List<string> files = Directory.GetFiles(@"E:\Users\Anwender\Desktop\⠀").ToList();
            List<string> files = Directory.GetFiles(dir).ToList();
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
                /*
                var item1 = item;
                if (item.StartsWith("~"))
                {
                    item1 = item.Remove(0, 2);
                }
                else
                {
                    File.Move(dir + item, dir + "~" + count + Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item));
                    count++;
                }*/

            /*

                if (ignoreFiles.Contains(System.IO.Path.GetFileName(item)))
                    continue;

                string text = System.IO.Path.GetFileName(item);
                CreateButtonAndAddToStackPanel(item, text);
            }
            */
            #endregion

            //

            //List<string>? orderedFiles = File.ReadAllLines("order")?.ToList();
            List<string>? orderedFiles;
            try
            {
                orderedFiles = File.ReadAllLines(Path.Combine(dir, "order"))?.ToList();
            }
            catch (Exception)
            {

                orderedFiles = null;
            }


            List<string> files = Directory.GetFiles(dir).ToList();
            files.AddRange(Directory.GetDirectories(dir));
            List<string> filescopy = Directory.GetFiles(dir).ToList();
            filescopy.AddRange(Directory.GetDirectories(dir));
            //files = files.OrderBy(f => Regex.Replace(f, @"~[\d-]", string.Empty)).ToList();
            //files = files. (f => f.Replace(dir, "")).ToList();

            for (int i = 0; i < filescopy.Count; i++)
            {
                //var f = filescopy[i];
                //f = f.Replace(dir, "");
                filescopy[i] = Path.GetFileName(filescopy[i]);
            }

            filescopy.Remove("order");


            /*
            bool newFile = false;


            //foreach (string file in filescopy)  
            if (orderedFiles != null)
                foreach (var item in orderedFiles)
                {
                    newFile = filescopy.Remove(item);
                    if (newFile) break;
                }


            //if (newFile)
            if (filescopy.Count > 0)
            {
                //orderedFiles.AddRange(filescopy);
                orderedFiles.AddRange(filescopy);
            }
            */

            //if (orderedFiles != null)
            //orderedFiles = filescopy.Except(orderedFiles).ToList();

            if (orderedFiles != null)
            {
                filescopy = filescopy.Except(orderedFiles).ToList();

                if (filescopy.Count > 0)
                {
                    orderedFiles.AddRange(filescopy);
                }
            }


            if (orderedFiles != null)
            {
                foreach (var fileName in orderedFiles)
                {
                    if (ignoreFiles.Contains(System.IO.Path.GetFileName(fileName)))
                        continue;

                    string filePath = Path.Combine(dir, fileName);

                    if (filePath.Contains("desktop.files.json"))
                    {
                        ;
                    }

                    if (File.Exists(filePath))
                        CreateButtonAndAddToStackPanel(filePath, fileName);
                }
            }
            else
            {
                int count = 0;
                foreach (string item in files)
                {
                    if (ignoreFiles.Contains(System.IO.Path.GetFileName(item)))
                        continue;

                    string text = System.IO.Path.GetFileName(item);
                    bool e = Directory.Exists(item);

                    //CreateButtonAndAddToStackPanel(item, text, fileName: e == false "cmd" : "explorer");
                    //var path = e == false ? "\"" + item + "\"" : "explorer \"" + item + "\"";
                    //var path = item;

                    CreateButtonAndAddToStackPanel(item, text, fileName: e == true ? "explorer" : "cmd");
                }
            }


        }

        private void CreatButtonsFromFiles2(List<string> ignoreFiles, string dir)
        {
            List<string>? orderedFiles;
            try
            {
                orderedFiles = File.ReadAllLines(Path.Combine(dir, "order"))?.ToList();
            }
            catch (Exception)
            {
                orderedFiles = null;
            }

            List<string> files = Directory.GetFiles(dir).ToList();
            files.AddRange(Directory.GetDirectories(dir));
            List<string> filescopy = Directory.GetFiles(dir).ToList();
            filescopy.AddRange(Directory.GetDirectories(dir));

            for (int i = 0; i < filescopy.Count; i++)
                filescopy[i] = Path.GetFileName(filescopy[i]);

            filescopy.Remove("order");

            if (orderedFiles != null)
            {
                filescopy = filescopy.Except(orderedFiles).ToList();

                if (filescopy.Count > 0)
                {
                    orderedFiles.AddRange(filescopy);
                }
            }

            if (orderedFiles != null)
            {
                foreach (var fileName in orderedFiles)
                {
                    if (ignoreFiles.Contains(System.IO.Path.GetFileName(fileName)))
                        continue;

                    string filePath = Path.Combine(dir, fileName);

                    if (filePath.Contains("desktop.files.json"))
                    {
                        ;
                    }

                    if (File.Exists(filePath))
                        CreateButtonAndAddToStackPanel(filePath, fileName);
                }
            }
            else
            {
                int count = 0;
                foreach (string item in files)
                {
                    if (ignoreFiles.Contains(System.IO.Path.GetFileName(item)))
                        continue;

                    string text = System.IO.Path.GetFileName(item);
                    bool e = Directory.Exists(item);

                    CreateButtonAndAddToStackPanel(item, text, fileName: e == true ? "explorer" : "cmd");
                }
            }


        }

        private void CreateButtonAndAddToStackPanel(string filePath, string text, string icon = "",
            string fileName = "cmd", bool hideIFActive = false, bool xml = false, bool onlyAddingBtn = false)
        {
            //ActionIconButton actionIconButton = new(text, false);

            ActionIconButton actionIconButton = new(text, Theme == WindowThemes.light ? true : false);
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


            if (xml)
                xml_sp.Children.Add(actionIconButton);
            else
                stackPanel.Children.Add(actionIconButton);


            ////this.Height = 18 + 33 * stackPanel.Children.Count;
            //this.Height = 18 + itemHeight * stackPanel.Children.Count + 4;
            //this.Width = 250;
            ////this.Background = actionIconButton.stackpanel.Background;

            this.Height = 18 + (itemHeight * stackPanel.Children.Count + 4) + (itemHeight * xml_sp.Children.Count + 4);
            //this.Width = 250;
            this.Width = 255;

            if (onlyAddingBtn)
                this.Top -= itemHeight;

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
                /*
                //processStartInfo.Arguments = "/c \"" + cmd + "\"";
                if (processStartInfo.FileName == "")
                    processStartInfo.FileName = cmd;
                else processStartInfo.Arguments = "/c " + cmd;
                //else processStartInfo.Arguments = "/c \"" + cmd + "\"";
                */
                if (btn.FileName == "explorer")
                {
                    processStartInfo.WorkingDirectory = dir;
                    processStartInfo.FileName = "explorer";
                    processStartInfo.Arguments = cmd;
                }
                //else if (processStartInfo.FileName == "")
                else
                {
                    //processStartInfo.FileName = cmd;
                    processStartInfo.Arguments = "/c " + cmd;
                }
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
            try
            {
                _isDown = false;
                _isDragging = false;
                _realDragSource?.ReleaseMouseCapture();
            }
            catch (Exception)
            {

                //throw;
            }

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
            var data = e.Data as DataObject;
            //e.Data.GetDataPresent()
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (var item in data.GetFileDropList())
                {
                    ;
                    //if (stackPanel.Children.Contains(ActionIconButton a => a.))

                    var fileName = Path.GetFileName(item);

                    if (stackPanel.Children.OfType<ActionIconButton>().Any(btn =>
                        btn.Text.Equals(fileName + ".lnk")))
                    {
                        continue;
                    }

                    Shortcuts.CreateShortcut(fileName, dir, item);
                    //LoadSettingsAndCreateButtons();
                    CreateButtonAndAddToStackPanel(Path.Combine(dir, fileName) + ".lnk", fileName + ".lnk",
                        onlyAddingBtn: true);
                }
            }

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

            //_isDown = false;
            //_isDragging = false;
            //_realDragSource.ReleaseMouseCapture();
        }

        private async void SaveNewOrder(ActionIconButton btn)
        {

            if (btn.FromXML)
            {
                /*
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
                */
            }
            else
            {
                CreateOrderFile(dir);
                CreateOrderFile(dir2);
            }


        }

        private void CreateOrderFile(string path)
        {
            //btn.FileName
            //File.copy
            //File.Move(btn.FileName, )

            int i = 0;
            //File file = File.CreateText("order");
            //using(StreamWriter sw = new StreamWriter(xmlFile))
            var filePath = Path.Combine(path, "order");

            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.Hidden);
                //await Task.Delay(100);
            }


            //using (StreamWriter sw = File.CreateText(Path.Combine(dir, fileName)))
            using (StreamWriter sw = File.CreateText(filePath))
            {

                foreach (ActionIconButton child in stackPanel.Children)
                {
                    if (!child.FromXML)
                        //sw.WriteLine(child.FileName);
                        sw.WriteLine(Path.GetFileName(child.Command));

                }

                File.SetAttributes(filePath, FileAttributes.Hidden);
            }

            //File.SetAttributes(Path.Combine(dir, "order"), FileAttributes.Hidden);

            /////
            ///
        }

        void OrderButtons()
        {

        }

        private void stackPanel_DragLeave(object sender, DragEventArgs e)
        {
            stackPanel_PreviewMouseLeftButtonUp(null, null);
        }

        private void Button_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine(sender + "dd");
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            //btn.Background = btn.Tag as SolidColorBrush;

            if (Theme == WindowThemes.dark)
            {
                btn.Foreground = (XmlSettings.Instance.GetValueOfSetting("lightFg") ?? "#000000").ToBrush();


            }
            else
            {
                btn.Foreground = (XmlSettings.Instance.GetValueOfSetting("darkFg") ?? "#FFFFFF").ToBrush();
            }

            //btn.Background = new SolidColorBrush(Colors.Red);
            //btn.Style.Setters.Add()
            //= new SolidColorBrush(Colors.Red);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = null;

            if (Theme == WindowThemes.light)
            {
                btn.Foreground = (XmlSettings.Instance.GetValueOfSetting("lightFg") ?? "#000000").ToBrush();


            }
            else
            {
                btn.Foreground = (XmlSettings.Instance.GetValueOfSetting("darkFg") ?? "#FFFFFF").ToBrush();
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start(dir);
            Process p = new();
            ProcessStartInfo i = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = "/c start " + dir,
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            p.StartInfo = i;
            p.Start();

        }
    }

    //class ColorInfo
    //{
    //    public SolidColorBrush hoverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
    //    public SolidColorBrush normal = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));

    //}
}