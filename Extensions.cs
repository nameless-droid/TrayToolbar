using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TrayToolbar
{
    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static SolidColorBrush ToBrush(this string s)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
        }
    }
}
