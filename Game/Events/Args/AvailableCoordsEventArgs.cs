using System.Collections.Generic;
using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Events.Args
{
    public class AvailableCoordsEventArgs
    {
        public List<Coords> AvailableCoords { get; set; }
    }
}