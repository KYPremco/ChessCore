using System.Collections.Generic;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Board
{
    /// <summary>
    /// Numeric to chess board locations
    /// </summary>
    public enum Coords
    {
        A8, B8, C8, D8, E8, F8, G8, H8,
        A7, B7, C7, D7, E7, F7, G7, H7,
        A6, B6, C6, D6, E6, F6, G6, H6,
        A5, B5, C5, D5, E5, F5, G5, H5,
        A4, B4, C4, D4, E4, F4, G4, H4,
        A3, B3, C3, D3, E3, F3, G3, H3,
        A2, B2, C2, D2, E2, F2, G2, H2,
        A1, B1, C1, D1, E1, F1, G1, H1,
    }

    public static class CoordsHelper
    {
        /// <summary>
        /// Finds out what row the coordinates are in
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static int Row(this Coords coords)
        {
            return (int) coords == 0 ? 0 : (int) coords / 8;
        }
        
        /// <summary>
        /// Finds out what row the coordinates are in with a reversed perspective
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public static int Row(this Coords coords, bool reversed)
        {
            if(reversed)
                return 7 - coords.Row();
            return coords.Row();
        }
        
        /// <summary>
        /// Finds out what column the coordinates are in
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static int Column(this Coords coords)
        {
            return (int) coords % 8;
        }

        /// <summary>
        /// Finds out what row the coordinates are in with a reversed perspective
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public static int Column(this Coords coords, bool reversed)
        {
            if(reversed)
                return 7 - coords.Column();
            return coords.Column();
        }
        
        /// <summary>
        /// Finds out if coordinate tile is white
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool IsWhiteTile(this Coords coords)
        {
            bool reverseTile = coords.Row() % 2 != 0;
            bool whiteTile = coords.Column() % 2 == 0;
            return reverseTile ? !whiteTile : whiteTile;
        }

        /// <summary>
        /// Is given coordinate inside the board
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool IsInsideBoard(this Coords coords)
        {
            return (int)coords >= 0 && (int)coords <= 63;
        }

        /// <summary>
        /// Has given coordinate a starting piece
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool HasStartingPiece(this Coords coords)
        {
            return StartingPoints().ContainsKey(coords);
        }
        
        /// <summary>
        /// Returns starting piece
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static Piece GetStartingPiece(this Coords coords)
        {
            return StartingPoints()[coords];
        }
        
        /// <summary>
        /// All starting positions
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Coords, Piece> StartingPoints()
        {
            Dictionary<Coords, Piece> dictionary = new Dictionary<Coords, Piece>();
            dictionary.Add(Coords.A8, new Rook(Coords.A8, PieceColor.Black));
            dictionary.Add(Coords.B8, new Knight(Coords.B8, PieceColor.Black));
            dictionary.Add(Coords.C8, new Bishop(Coords.C8, PieceColor.Black));
            dictionary.Add(Coords.D8, new King(Coords.D8, PieceColor.Black));
            dictionary.Add(Coords.E8, new Queen(Coords.E8, PieceColor.Black));
            dictionary.Add(Coords.F8, new Bishop(Coords.F8, PieceColor.Black));
            dictionary.Add(Coords.G8, new Knight(Coords.G8, PieceColor.Black));
            dictionary.Add(Coords.H8, new Rook(Coords.H8, PieceColor.Black));

            for (int i = (int)Coords.A7; i < (int)Coords.A7 + 8; i++)
            {
                dictionary.Add((Coords)i, new Pawn((Coords)i, PieceColor.Black));
            }
            
            for (int i = (int)Coords.A2; i < (int)Coords.A2 + 8; i++)
            {
                dictionary.Add((Coords)i, new Pawn((Coords)i, PieceColor.White));
            }

            dictionary.Add(Coords.A1, new Rook(Coords.A1, PieceColor.White));
            dictionary.Add(Coords.B1, new Knight(Coords.B1, PieceColor.White));
            dictionary.Add(Coords.C1, new Bishop(Coords.C1, PieceColor.White));
            dictionary.Add(Coords.D1, new King(Coords.D1, PieceColor.White));
            dictionary.Add(Coords.E1, new Queen(Coords.E1, PieceColor.White));
            dictionary.Add(Coords.F1, new Bishop(Coords.F1, PieceColor.White));
            dictionary.Add(Coords.G1, new Knight(Coords.G1, PieceColor.White));
            dictionary.Add(Coords.H1, new Rook(Coords.H1, PieceColor.White));
            
            return dictionary;
        }
    }
}