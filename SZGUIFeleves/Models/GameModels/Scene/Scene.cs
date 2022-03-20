using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Logic;

namespace SZGUIFeleves.Models
{
    public class Scene
    {
        public string Title { get; set; }

        public List<DrawableObject> Objects { get; set; }

        public int PlayerIndex { get; set; }
        public List<DynamicPointLight> PointLights { get; set; }

        public Scene(string title, List<DrawableObject> objects, int playerIndex, List<DynamicPointLight> pointLights)
        {
            Title = title;
            Objects = objects;
            PlayerIndex = playerIndex;
            PointLights = pointLights;
        }
    }
}
