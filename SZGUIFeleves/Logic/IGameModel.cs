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
        List<DrawableObject> ObjectsToDisplay { get; set; }
        Vec2d WindowSize { get; set; }
    }
}
