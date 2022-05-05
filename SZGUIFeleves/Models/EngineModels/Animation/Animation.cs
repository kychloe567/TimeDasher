using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public class Animation
    {
        public string Title { get; set; }
        [JsonIgnore]
        private List<BitmapImage> Textures { get; set; }
        [JsonProperty]
        private List<string> TexturePaths { get; set; }
        [JsonProperty]
        private List<double> Times { get; set; }    // Seconds

        private bool AnimationFixed { get; set; }
        private int StopAnimationOnIndex { get; set; }

        [JsonProperty]
        private int currentTexture;
        [JsonIgnore]
        public BitmapImage CurrentTexture
        {
            get { return Textures[currentTexture]; }
        }

        private DateTime Start { get; set; }

        public Animation()
        {
            Textures = new List<BitmapImage>();
            TexturePaths = new List<string>();
            Times = new List<double>();
            AnimationFixed = false;
        }
        public Animation(string title)
        {
            Title = title;
            Textures = new List<BitmapImage>();
            TexturePaths = new List<string>();
            Times = new List<double>();
            Start = DateTime.Now;
            AnimationFixed = false;
        }

        public Animation(string title, List<BitmapImage> textures, double time)
        {
            Title = title;
            Textures = textures;
            TexturePaths = new List<string>();
            for (int i = 0; i < Textures.Count; i++)
                Times.Add(time);
            Start = DateTime.Now;
            AnimationFixed = false;
        }
        public Animation(string title, List<BitmapImage> textures, List<double> times)
        {
            Title = title;
            Textures = textures;
            TexturePaths = new List<string>();
            Times = times;
            Start = DateTime.Now;
            AnimationFixed = false;
        }

        public void LoadTextures()
        {
            Textures = new List<BitmapImage>();

            foreach (string tex in TexturePaths)
                Textures.Add(new BitmapImage(new Uri(tex, UriKind.RelativeOrAbsolute)));
        }

        public void AddTexture(BitmapImage bi, string path, double time)
        {
            Textures.Add(bi);
            TexturePaths.Add(path);
            Times.Add(time);
        }

        public void StartAnimation(int currentTexture, bool fix, int stopOn)
        {
            this.currentTexture = currentTexture;
            StopAnimationOnIndex = stopOn;

            AnimationFixed = fix;
            Start = DateTime.Now;
        }

        public void UpdateAnimation()
        {
            if(!AnimationFixed && StopAnimationOnIndex != currentTexture && (DateTime.Now-Start).TotalSeconds >= Times[currentTexture])
            {
                currentTexture++;
                if (currentTexture == Times.Count)
                    currentTexture = 0;
                Start = DateTime.Now;
            }
        }

        public Animation GetCopy()
        {
            List<BitmapImage> textures = new List<BitmapImage>();
            foreach (BitmapImage bi in Textures)
                textures.Add(bi.Clone());

            return new Animation(Title, textures, new List<double>(Times))
            {
                currentTexture = currentTexture,
                TexturePaths = new List<string>(TexturePaths)
            };
        }
    }
}
