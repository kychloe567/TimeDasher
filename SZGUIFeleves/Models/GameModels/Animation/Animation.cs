using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public class Animation
    {
        public string Title { get; set; }
        private List<BitmapImage> Textures { get; set; }
        private List<double> Times { get; set; }    // Seconds

        private int currentTexture;
        public BitmapImage CurrentTexture
        {
            get { return Textures[currentTexture]; }
        }

        private DateTime Start { get; set; }

        public Animation(string title)
        {
            Title = title;
            Textures = new List<BitmapImage>();
            Times = new List<double>();
        }

        public Animation(string title, List<BitmapImage> textures, double time)
        {
            Title = title;
            Textures = textures;
            for (int i = 0; i < Textures.Count; i++)
                Times.Add(time);
        }
        public Animation(string title, List<BitmapImage> textures, List<double> times)
        {
            Title = title;
            Textures = textures;
            Times = times;
        }

        public void AddTexture(BitmapImage bi, double time)
        {
            Textures.Add(bi);
            Times.Add(time);
        }

        public void StartAnimation()
        {
            currentTexture = 0;
            Start = DateTime.Now;
        }

        public void UpdateAnimation()
        {
            if((DateTime.Now-Start).TotalSeconds >= Times[currentTexture])
            {
                currentTexture++;
                if (currentTexture == Times.Count)
                    currentTexture = 0;
                Start = DateTime.Now;
            }
        }
    }
}
