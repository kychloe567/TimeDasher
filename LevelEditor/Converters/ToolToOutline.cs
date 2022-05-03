using LevelEditor.Logic;
using LevelEditor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LevelEditor.Converters
{
    public class ToolToOutline : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Tool tool = (Tool)value;

            if (parameter.ToString() == "move" && tool == Tool.Move ||
                parameter.ToString() == "selection" && tool == Tool.Selection ||
                parameter.ToString() == "player" && tool == Tool.Player)
                return Brushes.Gray;

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e1e1e"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
