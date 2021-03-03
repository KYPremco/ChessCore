using System.Collections.Generic;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Board
{
    public class Board
    {
        internal King WhiteKing { get; set; }
        
        internal List<Piece> WhitePieces { get; }
        
        public List<Piece> WhitePiecesTaken { get; private set; }
        
        internal King BlackKing { get; set; }
        
        internal List<Piece> BlackPieces { get; }
        
        public List<Piece> BlackPiecesTaken { get; private set; }
        
        public Tile[] Tiles { get; }

        public Board()
        {
            Tiles = new Tile[64];
            WhitePieces = new List<Piece>();
            WhitePiecesTaken = new List<Piece>();
            BlackPieces = new List<Piece>();
            BlackPiecesTaken = new List<Piece>();
        }
    }
}