using System.Collections.Generic;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Board
{
    public class Board
    {
        internal King WhiteKing { get; set; }
        
        internal List<Piece> WhitePieces { get; }

        internal King BlackKing { get; set; }
        
        internal List<Piece> BlackPieces { get; }

        public Tile[] Tiles { get; }

        public Board()
        {
            Tiles = new Tile[64];
            WhitePieces = new List<Piece>();
            BlackPieces = new List<Piece>();
        }
    }
}