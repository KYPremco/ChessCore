using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Events.Args
{
    public class PieceTakenOverArgs
    {
        public Piece TakenPiece { get; set; }
        
        public Coords TakenCoords { get; set; }
    }
}