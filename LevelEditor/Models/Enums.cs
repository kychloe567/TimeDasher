using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Models
{
    public enum Tool
    {
        Place, Move, Selection, Player, Checkpoint, End
    }

    public enum ButtonKey // TODO: More buttons to add if needed
    {
        W, A, S, D,
        Up, Down, Left, Right,
        Space, C, LeftCtrl, Delete,
        Q, E,
        MouseLeft, MouseRight, MouseMiddle
    }
}
