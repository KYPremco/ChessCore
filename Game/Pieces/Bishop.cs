using System;
using System.Collections.Generic;
using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Pieces
{
    public class Bishop : Piece
    {
        public override EPiece EPiece { get; } = EPiece.Bishop;

        internal Bishop(Coords coords, Player player) : base(coords, player)
        {
            
        }

        protected override List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            coordsList.AddRange(HandleCoords(board, 0, (c, r) => c + r * 7, xRay, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, 7, (c, r) => c + r * 9, xRay, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, 7, (c, r) => c - r * 7, xRay, updateBlockingPieces));
            coordsList.AddRange(HandleCoords(board, 0, (c, r) => c - r * 9, xRay, updateBlockingPieces));
            
            return coordsList;
        }

        private List<Coords> HandleCoords(Tile[] board, int corner, Func<int, int, int> op, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();
            bool finished = false;
            int rotation = 1;

            while (!finished)
            {
                Coords coordinate = (Coords) op((int) Coords, rotation);
                
                if(!ValidCoordinate(board, coordinate, corner, true))
                    break;
                
                if (board[(int) coordinate].Piece?.Side == Side)
                {
                    if(!xRay)
                        break;
                    finished = true;
                }

                if (board.HasPawnOnTile(coordinate) && board[(int) coordinate].Piece.Side != Side)
                {
                    if (updateBlockingPieces)
                    {
                        if(coordinate.Column() != corner)
                            HandleBlocking(board, board[(int) coordinate].Piece, op, corner, rotation + 1);
                        HandleTargetKing(board, op, corner, rotation);
                    }
                    finished = true;
                }
                    

                board[(int) coordinate].IsActive = true;
                coordsList.Add(coordinate);

                if (coordinate.Column() == corner)
                    finished = true;

                rotation++;
            }
            
            return FilterKingProtectionCoords(coordsList);
        }
    }
}