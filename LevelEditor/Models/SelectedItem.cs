using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SZGUIFeleves.Models;

namespace LevelEditor.Models
{
    public class SelectedItem
    {
        public DrawableObject Object { get; set; }
        public BitmapImage SelectedTexture { get; set; }
        public BitmapImage SelectedTextureRed { get; set; }
        public BitmapImage SelectedTextureGreen { get; set; }

        public SelectedItem() { }
    }
}
