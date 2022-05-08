using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models.DrawableObjects;
using SZGUIFeleves.Models.EngineModels.MovingBackground;

namespace SZGUIFeleves.Models
{
    class SceneJson
    {
        public string Title { get; set; }
        public List<Circle> Circles { get; set; }
        public List<Line> Lines { get; set; }
        public List<Polygon> Polygons { get; set; }
        public List<Trap> Traps { get; set; }
        public List<Rectangle> Rectangles { get; set; }
        public List<Text> Texts { get; set; }
        public List<DynamicPointLight> PointLights { get; set; }

        public MovingBackground MovingBackground { get; set; }
        public Player Player { get; set; }

        public SceneJson()
        {
            Circles = new List<Circle>();
            Lines = new List<Line>();
            Polygons = new List<Polygon>();
            Traps = new List<Trap>();
            Rectangles = new List<Rectangle>();
            Texts = new List<Text>();
            MovingBackground = new MovingBackground();
        }
    }
}
