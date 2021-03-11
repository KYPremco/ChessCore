using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Events.Args
{
    public class PawnUpgradedArgs
    {
        public Piece NewPiece { get; set; }
        
        public Coords Coords { get; set; }
    }
}