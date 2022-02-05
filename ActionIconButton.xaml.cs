using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrayToolbar
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ActionIconButton : UserControl
    {
        public string Command { get; set; }
        public bool LightMode { get; set; }


        private Brush oldBg;

        public ActionIconButton(string text)
        {
            InitializeComponent();

            Text = text;
            lbl.Content = text;

            this.DataContext = this;
            this.MouseEnter += UserControl1_MouseEnter;
            this.MouseLeave += UserControl1_MouseLeave;

            PreviewMouseLeftButtonUp += (sender, args) => OnClick();
        }

        public void UpdateLightDarkMode(bool light)
        {
            if (light == true)
            {
                lbl.Foreground = Brushes.Black;
            }
            else
            {
                lbl.Foreground = Brushes.White;
            }
            LightMode = light;
        }

        private void UserControl1_MouseEnter(object sender, MouseEventArgs e)
        {
            /*
            //#E5F3FF
            //stackpanel.Background.Opacity = 0.6;
            oldBg = stackpanel.Background;
            //this.Background.Opacity = 0.5;
            Color oldBGCol = ((System.Windows.Media.SolidColorBrush)(oldBg)).Color;
            //Color col = ChangeLightness(new SolidColorBrush(oldBGCol).Color, 10);
            //Color col = ChangeLightness(oldBGCol, 5);
            Color col = ChangeLightness(oldBGCol, 2);
            stackpanel.Background = new SolidColorBrush(col);
            */
            MouseOver();
        }

        private void MouseOver()
        {
            if (LightMode == true)
            {
                //#EEEEEE
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            }
            else
            {
                //#2B2B2B
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#414141"));
            }
        }

        private void UserControl1_MouseLeave(object sender, MouseEventArgs e)
        {
            /*
            //stackpanel.Background.Opacity = 1;
            //this.Background.Opacity = 1;

            stackpanel.Background = oldBg;
            */
            MouseNotOver();
        }

        private void MouseNotOver()
        {
            if (LightMode == true)
            {
                //#FFFFFF
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
            }
            else
            {
                //#414141
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
            }
        }

        private const int MinLightness = 1;
        private const int MaxLightness = 10;
        private const float MinLightnessCoef = 1f;
        private const float MaxLightnessCoef = 0.4f;

        public Color ChangeLightness(Color color, int lightness)
        {
            if (lightness < MinLightness)
                lightness = MinLightness;
            else if (lightness > MaxLightness)
                lightness = MaxLightness;

            float coef = MinLightnessCoef +
              (
                (lightness - MinLightness) *
                  ((MaxLightnessCoef - MinLightnessCoef) / (MaxLightness - MinLightness))
              );

            return Color.FromArgb(color.A, (byte)(int)(color.R * coef), (byte)(int)(color.G * coef),
                (byte)(int)(color.B * coef));
        }

        public Brushes background;

        public ImageSource icon;

        public string Icon
        {
            set
            {
                SetIconFromFile(value);
                //lbl.Content = System.IO.Path.GetFileName(value);
            }
        }

        public string BG
        {
            set
            {
                //stackpanel.Background = new SolidColorBrush(Color));
                stackpanel.Background = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(value);
            }
        }

        public void SetIconFromFile(string filePath)
        {
            //filePath.Normalize();

            if (filePath.StartsWith("\\"))
            {
                ;
            }

            if (filePath.StartsWith("//"))
            {
                ;
            }

            if (filePath.StartsWith("\""))
            {
                ;
                //filePath = filePath.Substring(2);
                filePath = filePath.TrimStart(filePath[0]);
            }

            if (filePath.EndsWith("\""))
            {
                ;
                //filePath = filePath.Substring(2);
                filePath = filePath.TrimEnd(filePath[^1]);
            }

            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            img.Source = ToImageSource(icon);
        }

        public ImageSource ToImageSource(System.Drawing.Icon icon)
        {
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public string Text { get; set; }

        public void SetText(string value)
        {

        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ActionIconButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        void RaiseClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ActionIconButton.ClickEvent);
            RaiseEvent(newEventArgs);
        }

        void OnClick()
        {
            RaiseClickEvent();
        }

        private void ucontrol_Loaded(object sender, RoutedEventArgs e)
        {

        }



        /////

        ///// <summary>
        ///// Create a custom routed event by first registering a RoutedEventID
        ///// This event uses the bubbling routing strategy
        ///// see the web page https://msdn.microsoft.com/EN-US/library/vstudio/ms598898(v=vs.90).aspx 
        ///// </summary>
        //public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UserControl1));
        ///// <summary>
        ///// Provide CLR accessors for the event Click OPButton 
        ///// Adds a routed event handler for a specified routed event Click, adding the handler to the handler collection on the current element.
        ///// </summary>
        //public event RoutedEventHandler Click
        //{
        //    add { AddHandler(ClickEvent, value); }
        //    remove { RemoveHandler(ClickEvent, value); }
        //}
        ///// <summary>
        ///// This method raises the Click event 
        ///// </summary>
        //private void RaiseClickEvent()
        //{
        //    RoutedEventArgs newEventArgs = new RoutedEventArgs(UserControl1.ClickEvent);
        //    RaiseEvent(newEventArgs);
        //}
        ///// <summary>
        ///// For isPressed purposes we raise the event when the OPButton is clicked
        ///// </summary>
        //private void OnClick()
        //{
        //    RaiseClickEvent();
        //}
    }
}
