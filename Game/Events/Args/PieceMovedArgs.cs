using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Events.Args
{
    public class PieceMovedArgs
    {
        public Piece MovedPiece { get; set; }
        
        public Coords FromCoords { get; set; }
        
        public Coords NewCoords { get; set; }
    }
}