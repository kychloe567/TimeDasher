using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models.EngineModels.MovingBackground
{
    public class MovingBackground
    {
        public string Set { get; set; }

        public string BackgroundPath { get; set; }

        public string FarPath { get; set; }
        public string MiddlePath { get; set; }
        public string ClosePath { get; set; }

        [JsonIgnore]
        public BitmapImage Background { get; set; }
        [JsonIgnore]
        public BitmapImage Far { get; set; }
        [JsonIgnore]
        public BitmapImage Middle { get; set; }
        [JsonIgnore]
        public BitmapImage Close { get; set; }

        public double BackgroundPosition { get; set; }
        public double FarPosition { get; set; }
        public double MiddlePosition { get; set; }
        public double ClosePosition { get; set; }

        public MovingBackground()
        {
            LoadTextures();
        }

        public void LoadTextures()
        {
            if (!(BackgroundPath is null) && BackgroundPath != "")
            {
                Background = new BitmapImage(new Uri(BackgroundPath, UriKind.RelativeOrAbsolute));
                Far = new BitmapImage(new Uri(FarPath, UriKind.RelativeOrAbsolute));
                Middle = new BitmapImage(new Uri(MiddlePath, UriKind.RelativeOrAbsolute));
                Close = new BitmapImage(new Uri(ClosePath, UriKind.RelativeOrAbsolute));
            }
        }

        public List<DrawableObject> GetDefault(Vec2d WindowSize)
        {
            List<DrawableObject> bg = new List<DrawableObject>();

            bg.Add(new Rectangle(new Vec2d(), WindowSize)
            {
                Texture = Background
            });
            bg.Add(new Rectangle(new Vec2d(), WindowSize)
            {
                Texture = Far
            });
            bg.Add(new Rectangle(new Vec2d(), WindowSize)
            {
                Texture = Middle
            });
            bg.Add(new Rectangle(new Vec2d(), WindowSize)
            {
                Texture = Close
            });

            return bg;
        }

        public List<DrawableObject> UpdateBackground(Vec2d WindowSize)
        {
            List<DrawableObject> bg = new List<DrawableObject>();

            // Background
            if (BackgroundPosition > WindowSize.x)
            {
                while (BackgroundPosition > WindowSize.x)
                    BackgroundPosition -= WindowSize.x;
            }
            else if (BackgroundPosition < -WindowSize.x)
            {
                while (BackgroundPosition < -WindowSize.x)
                    BackgroundPosition += WindowSize.x;
            }
            if (BackgroundPosition == 0 || BackgroundPosition % WindowSize.x == 0)
            {
                bg.Add(new Rectangle(new Vec2d(), WindowSize)
                {
                    Texture = Background
                });
            }
            else if (BackgroundPosition < 0)
            {
                bg.Add(new Rectangle(new Vec2d(BackgroundPosition, 0), WindowSize)
                {
                    Texture = Background
                });
                bg.Add(new Rectangle(new Vec2d(BackgroundPosition + WindowSize.x - 1, 0), WindowSize)
                {
                    Texture = Background
                });
            }
            else
            {
                bg.Add(new Rectangle(new Vec2d(BackgroundPosition, 0), WindowSize)
                {
                    Texture = Background
                });
                bg.Add(new Rectangle(new Vec2d(BackgroundPosition - WindowSize.x + 1, 0), WindowSize)
                {
                    Texture = Background
                });
            }

            // Far
            if (FarPosition > WindowSize.x)
            {
                while (FarPosition > WindowSize.x)
                    FarPosition -= WindowSize.x;
            }
            else if(FarPosition < -WindowSize.x)
            {
                while (FarPosition < -WindowSize.x)
                    FarPosition += WindowSize.x;
            }
            if(FarPosition == 0 || FarPosition % WindowSize.x == 0)
            {
                bg.Add(new Rectangle(new Vec2d(), WindowSize)
                {
                    Texture = Far
                });
            }
            else if(FarPosition < 0)
            {
                bg.Add(new Rectangle(new Vec2d(FarPosition, 0), WindowSize)
                {
                    Texture = Far
                });
                bg.Add(new Rectangle(new Vec2d(FarPosition + WindowSize.x - 1, 0), WindowSize)
                {
                    Texture = Far
                });
            }
            else
            {
                bg.Add(new Rectangle(new Vec2d(FarPosition, 0), WindowSize)
                {
                    Texture = Far
                });
                bg.Add(new Rectangle(new Vec2d(FarPosition - WindowSize.x + 1, 0), WindowSize)
                {
                    Texture = Far
                });
            }

            // Middle
            if (MiddlePosition > WindowSize.x)
            {
                while (MiddlePosition > WindowSize.x)
                    MiddlePosition -= WindowSize.x;
            }
            else if (MiddlePosition < -WindowSize.x)
            {
                while (MiddlePosition < -WindowSize.x)
                    MiddlePosition += WindowSize.x;
            }
            if (MiddlePosition == 0 || MiddlePosition % WindowSize.x == 0)
            {
                bg.Add(new Rectangle(new Vec2d(), WindowSize)
                {
                    Texture = Middle
                });
            }
            else if(MiddlePosition < 0)
            {
                bg.Add(new Rectangle(new Vec2d(MiddlePosition, 0), WindowSize)
                {
                    Texture = Middle
                });
                bg.Add(new Rectangle(new Vec2d(MiddlePosition + WindowSize.x - 1, 0), WindowSize)
                {
                    Texture = Middle
                });
            }
            else
            {
                bg.Add(new Rectangle(new Vec2d(MiddlePosition, 0), WindowSize)
                {
                    Texture = Middle
                });
                bg.Add(new Rectangle(new Vec2d(MiddlePosition - WindowSize.x + 1, 0), WindowSize)
                {
                    Texture = Middle
                });
            }

            // Close
            if (ClosePosition > WindowSize.x)
            {
                while (ClosePosition > WindowSize.x)
                    ClosePosition -= WindowSize.x;
            }
            else if (ClosePosition < -WindowSize.x)
            {
                while (ClosePosition < -WindowSize.x)
                    ClosePosition += WindowSize.x;
            }
            if (ClosePosition == 0 || ClosePosition % WindowSize.x == 0)
            {
                bg.Add(new Rectangle(new Vec2d(), WindowSize)
                {
                    Texture = Close
                });
            }
            else if(ClosePosition < 0)
            {
                bg.Add(new Rectangle(new Vec2d(ClosePosition, 0), WindowSize)
                {
                    Texture = Close
                });
                bg.Add(new Rectangle(new Vec2d(ClosePosition + WindowSize.x - 1, 0), WindowSize)
                {
                    Texture = Close
                });
            }
            else
            {
                bg.Add(new Rectangle(new Vec2d(ClosePosition, 0), WindowSize)
                {
                    Texture = Close
                });
                bg.Add(new Rectangle(new Vec2d(ClosePosition - WindowSize.x + 1, 0), WindowSize)
                {
                    Texture = Close
                });
            }


            return bg;
        }
    }
}
