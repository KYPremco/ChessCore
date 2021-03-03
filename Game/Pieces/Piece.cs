using System;
using System.Collections.Generic;
using System.Linq;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Struct;

namespace OnlineChessCore.Game.Pieces
{
    public abstract class Piece
    {
        protected Coords Coords { get; set; }
        
        public virtual EPiece EPiece { get; }
        
        public Player Side { get; }
        
        internal List<Attack> Blocking { get; }

        protected internal Piece(Coords coords, Player side)
        {
            Coords = coords;
            Side = side;
            Blocking = new List<Attack>();
        }
        
        protected abstract List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces);

        internal List<Coords> AvailableCoords(Tile[] board, bool xRay = false)
        {
            return AvailableCoords(board, xRay, false);
        }
        
        internal void UpdateAvailableCoords(Tile[] board)
        {
            AvailableCoords(board, false, true);
        }
        
        protected List<Coords> FilterKingProtectionCoords(List<Coords> availableCoordsList)
        {
            if(Blocking.Count < 1)
                return availableCoordsList;

            return Blocking
                .Select(x => x.Coordinate)
                .Intersect(availableCoordsList)
                .ToList();
        }
        
        protected bool ValidCoordinate(Tile[] board, Coords coords, int sideCorner, bool ignoreOwnPiece = false)
        {
            if (!coords.IsInsideBoard())
                return false;
                
            if(sideCorner != -1 && Coords.Column() == sideCorner)
                return false;

            if(!ignoreOwnPiece && board.HasPawnOnTile(coords) && board[(int) coords].Piece.Side == Side && board[(int) coords].Piece != this)
                return false;

            return true;
        }
        
        protected void HandleBlocking(Tile[] board, Piece blockingPiece, Func<int, int, int> op, int corner, int rotation)
        {
            while (true)
            {
                Coords coordinate = (Coords) op((int) Coords, rotation);
                
                if(!ValidCoordinate(board, coordinate, corner))
                    return;
                

                if (board.HasPawnOnTile(coordinate))
                {
                    if(board[(int) coordinate].Piece.EPiece == EPiece.King)
                        blockingPiece.Blocking.AddRange(GetAttackLine(board, op, rotation, corner));
                    return;
                }
                
                rotation++;
            }
        }

        protected void HandleTargetKing(Tile[] board, Coords coordinate)
        {
            if (!board.HasPawnOnTile(coordinate) || board[(int) coordinate]?.Piece.EPiece != EPiece.King)
                return;
            
            ((King) board[(int) coordinate].Piece).checks.Add(new Attack(Coords, false));
        }
        
        protected void HandleTargetKing(Tile[] board, Func<int, int, int> op, int corner, int rotation)
        {
            if (board[op((int) Coords, rotation)].Piece.EPiece != EPiece.King)
                return;

            ((King) board[op((int) Coords, rotation)].Piece).checks.AddRange(GetAttackLine(board, op, rotation, corner));
        }
        
        private List<Attack> GetAttackLine(Tile[] board, Func<int, int, int> op, int rotation, int corner)
        {
            List<Attack> attackList = new List<Attack>();

            bool finished = false;
            bool outOfBounds = false;
            bool ghost = true;
            while (!finished)
            {
                Coords coordinate = (Coords) op((int) Coords, rotation);

                if (!ValidCoordinate(board, coordinate, corner) || coordinate.Column() == corner)
                {
                    outOfBounds = true;
                    rotation--;
                    continue;
                }

                if (board.HasPawnOnTile(coordinate))
                {
                    if (outOfBounds && board[(int) coordinate].Piece.EPiece == EPiece.King)
                        ghost = false;
                    
                    if(board[(int) coordinate].Piece == this)
                        finished = true;
                }
                    

                attackList.Add(new Attack(coordinate, ghost));
                rotation = outOfBounds ? rotation - 1 : rotation + 1;
            }

            return attackList.Distinct().ToList();
        }

        internal virtual void Move(Coords coords)
        {
            Coords = coords;
        }
    }
}