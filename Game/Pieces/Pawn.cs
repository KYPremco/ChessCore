using System;
using System.Collections.Generic;
using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Pieces
{
    public class Pawn : Piece
    {
        public override EPiece EPiece { get; } = EPiece.Pawn;

        public Pawn(Coords coords, Player player) : base(coords, player)
        {

        }

        protected override List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            if (Side == Player.White)
            {
                coordsList.AddRange(HandleCoords(board, -1, false, (c) => c - 8, updateBlockingPieces));
                coordsList.AddRange(HandleCoords(board, 7, true, (c) => c - 7, updateBlockingPieces));
                coordsList.AddRange(HandleCoords(board, 0, true, (c) => c - 9, updateBlockingPieces));

            }
            else
            {
                coordsList.AddRange(HandleCoords(board, -1, false, (c) => c + 8, updateBlockingPieces));
                coordsList.AddRange(HandleCoords(board, 0, true, (c) => c + 7, updateBlockingPieces));
                coordsList.AddRange(HandleCoords(board, 7, true, (c) => c + 9, updateBlockingPieces));
            }

            return coordsList;
        }

        private List<Coords> HandleCoords(Tile[] board, int corner, bool takeOver, Func<int, int> op, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            if (!ValidCoordinate(board, (Coords) op((int) Coords), corner))
                return FilterKingProtectionCoords(coordsList);
            
            if(takeOver ^ board.HasPawnOnTile((Coords) op((int) Coords)))
                return FilterKingProtectionCoords(coordsList);

            board[op((int) Coords)].IsActive = true;
            coordsList.Add((Coords) op((int) Coords));
            
            if(updateBlockingPieces)
                HandleTargetKing(board, (Coords) op((int) Coords));
            return FilterKingProtectionCoords(coordsList);
        }
    }
}