using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using SZGUIFeleves.Models;

namespace LevelEditor.Converters
{
    public class DrawLevelToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DrawPriority dp = value as DrawPriority;

            if (parameter.ToString() == "top" && dp.Type == DrawPriority.PriorityType.Top ||
                parameter.ToString() == "custom" && dp.Type == DrawPriority.PriorityType.Custom ||
                parameter.ToString() == "bottom" && dp.Type == DrawPriority.PriorityType.Bottom)
                return System.Windows.Media.Brushes.LightGreen;
            //return SystemColors.ControlLightLight;
            return System.Windows.Media.Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
