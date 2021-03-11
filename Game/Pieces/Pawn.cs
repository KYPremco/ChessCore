using System;
using System.Collections.Generic;
using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Pieces
{
    public class Pawn : Piece
    {
        public override EPiece EPiece { get; } = EPiece.Pawn;
        
        private bool HasMoved { get; set; }

        public Pawn(Coords coords, PieceColor pieceColor) : base(coords, pieceColor)
        {

        }

        protected override List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            if (Side == PieceColor.White)
            {
                coordsList.AddRange(HandleCoords(board, -1, false, (c) => c - 8, updateBlockingPieces, xRay));
                coordsList.AddRange(HandleCoords(board, 7, true, (c) => c - 7, updateBlockingPieces, xRay));
                coordsList.AddRange(HandleCoords(board, 0, true, (c) => c - 9, updateBlockingPieces, xRay));
                
                if(!HasMoved)
                    coordsList.AddRange(HandleCoords(board, -1, false, (c) => c - 16, updateBlockingPieces, xRay));
            }
            else
            {
                coordsList.AddRange(HandleCoords(board, -1, false, (c) => c + 8, updateBlockingPieces, xRay));
                coordsList.AddRange(HandleCoords(board, 0, true, (c) => c + 7, updateBlockingPieces, xRay));
                coordsList.AddRange(HandleCoords(board, 7, true, (c) => c + 9, updateBlockingPieces, xRay));
                
                if(!HasMoved)
                    coordsList.AddRange(HandleCoords(board, -1, false, (c) => c + 16, updateBlockingPieces, xRay));
            }

            return coordsList;
        }

        /// <summary>
        /// Reusable method for calculating valid position within the board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="corner"></param>
        /// <param name="takeOver"></param>
        /// <param name="op"></param>
        /// <param name="updateBlockingPieces"></param>
        /// <param name="xRay">King can't take over if this pawn is targeted by its own piece</param>
        /// <returns></returns>
        private List<Coords> HandleCoords(Tile[] board, int corner, bool takeOver, Func<int, int> op, bool updateBlockingPieces, bool xRay)
        {
            List<Coords> coordsList = new List<Coords>();

            if (!ValidCoordinate(board, (Coords) op((int) Coords), corner, xRay))
                return FilterKingProtectionCoords(coordsList);
            
            if(takeOver ^ board.HasPawnOnTile((Coords) op((int) Coords)))
                return FilterKingProtectionCoords(coordsList);
            
            coordsList.Add((Coords) op((int) Coords));
            
            if(updateBlockingPieces)
                HandleTargetKing(board, (Coords) op((int) Coords));
            return FilterKingProtectionCoords(coordsList);
        }
        
        /// <summary>
        /// Rook has switched from original place
        /// </summary>
        /// <param name="coords"></param>
        internal override void Move(Coords coords)
        {
            Coords = coords;
            HasMoved = true;
        }
    }
}