using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SZGUIFeleves.ViewModels;

namespace SZGUIFeleves.Converters
{
    public class GameStateToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GameStates b = (GameStates)value;
            string p = (string)parameter;

            if (b == GameStates.Pause && (p == "game" || p == "pause"))
                return Visibility.Visible;
            else if ((b == GameStates.Menu && p == "menu") ||
                (b == GameStates.Game && p == "game") ||
                (b == GameStates.Leaderboard && p == "leaderboard") ||
                (b == GameStates.LevelSelection && p == "levelselection"))
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
