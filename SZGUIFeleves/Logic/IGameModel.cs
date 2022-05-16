using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Logic
{
    public interface IGameModel
    {
        event DrawDelegate DrawEvent;
        List<DrawableObject> ObjectsToDisplayScreenSpace { get; set; }
        List<DrawableObject> ObjectsToDisplayWorldSpace { get; set; }
        List<DrawableObject> MovingBackgrounds { get; set; }
        Vec2d PlayerPosition { get; set; }
        bool IsThereShadow { get; set; }
        Camera Camera { get; set; }
    }
}
