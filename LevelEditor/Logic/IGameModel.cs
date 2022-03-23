using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models;

namespace LevelEditor.Logic
{
    public interface IGameModel
    {
        event DrawDelegate DrawEvent;
        List<DrawableObject> ObjectsToDisplayWorldSpace { get; set; }
        Camera Camera { get; set; }
    }
}
