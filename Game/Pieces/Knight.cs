using System;
using System.Collections.Generic;
using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Pieces
{
    public class Knight : Piece
    {
        public override EPiece EPiece { get; } = EPiece.Knight;
        
        public Knight(Coords coords, Player player): base(coords, player)
        {

        }

        protected override List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();
            List<int> rightCorner = new List<int>() {6, 7};
            List<int> leftCorner = new List<int>() {0, 1};


            coordsList.AddRange(HandleCoords(board, new List<int>() {0}, (c) => c + 15, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, new List<int>() {7}, (c) => c + 17, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, new List<int>() {7}, (c) => c - 15, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, new List<int>() {0}, (c) => c - 17, updateBlockingPieces));

            coordsList.AddRange(HandleCoords(board, leftCorner, (c) => c + 6, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, rightCorner, (c) => c + 10, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, rightCorner, (c) => c - 6, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, leftCorner, (c) => c - 10, updateBlockingPieces));

            return coordsList;
        }

        /// <summary>
        /// Reusable method for calculating valid position within the board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="corner"></param>
        /// <param name="op"></param>
        /// <param name="updateBlockingPieces"></param>
        /// <returns></returns>
        private List<Coords> HandleCoords(Tile[] board, ICollection<int> corner, Func<int, int> op, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            foreach (int sideCorner in corner)
            {
                if(!ValidCoordinate(board, (Coords) op((int) Coords), sideCorner))
                    return FilterKingProtectionCoords(coordsList);
            }
            
            coordsList.Add((Coords) op((int) Coords));
            
            if(updateBlockingPieces)
                HandleTargetKing(board, (Coords) op((int) Coords));
            return FilterKingProtectionCoords(coordsList);
        }
    }
}