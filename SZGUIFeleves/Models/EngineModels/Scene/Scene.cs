using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Logic;
using SZGUIFeleves.Models.EngineModels.MovingBackground;

namespace SZGUIFeleves.Models
{
    public class Scene
    {
        public string Title { get; set; }

        public List<DrawableObject> Objects { get; set; }
        public MovingBackground MovingBackground { get; set; }

        public int PlayerIndex { get; set; }
        public List<DynamicPointLight> PointLights { get; set; }
        public double LowestPoint { get; set; }
        public List<Rectangle> MergedForeground { get; set; }

        public Scene(string title, List<DrawableObject> objects, int playerIndex, List<DynamicPointLight> pointLights, 
                     MovingBackground movingBackground, List<Rectangle> merged)
        {
            Title = title;
            Objects = objects;
            PlayerIndex = playerIndex;
            PointLights = pointLights;
            MovingBackground = movingBackground;
            MergedForeground = merged;
        }
    }
}
