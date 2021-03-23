using System;
using System.Collections.Generic;
using System.Linq;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Struct;

namespace OnlineChessCore.Game.Pieces
{
    public abstract class Piece
    {
        internal Coords Coords { get; set; }
        
        public virtual EPiece EPiece { get; }
        
        public PieceColor Side { get; }
        
        internal List<AttackCoord> Blocking { get; }

        protected internal Piece(Coords coords, PieceColor side)
        {
            Coords = coords;
            Side = side;
            Blocking = new List<AttackCoord>();
        }
        
        /// <summary>
        /// Abstract method for receiving available coords without rules for given piece
        /// </summary>
        /// <param name="board"></param>
        /// <param name="xRay">Adds available coord to take over own pieces</param>
        /// <param name="updateBlockingPieces">Will trigger @HandlingBlocking @HandleKingTarget</param>
        /// <returns></returns>
        protected abstract List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces);

        /// <summary>
        /// Get available coords for given piece
        /// </summary>
        /// <param name="board"></param>
        /// <param name="xRay">Adds available coord to take over own pieces</param>
        /// <returns></returns>
        internal List<Coords> AvailableCoords(Tile[] board, bool xRay = false)
        {
            return AvailableCoords(board, xRay, false);
        }
        
        /// <summary>
        /// Updates blocking and targeting, @HandleBlocking & HandleTargetKing
        /// </summary>
        /// <param name="board"></param>
        internal void UpdateAvailableCoords(Tile[] board)
        {
            AvailableCoords(board, false, true);
        }
        
        /// <summary>
        /// Filters out normal coords when a piece is blocking, can only move in same direction or taking over attacking piece
        /// </summary>
        /// <param name="availableCoordsList"></param>
        /// <returns></returns>
        protected List<Coords> FilterKingProtectionCoords(List<Coords> availableCoordsList)
        {
            if(Blocking.Count < 1)
                return availableCoordsList;

            return Blocking
                .Select(x => x.Coordinate)
                .Intersect(availableCoordsList)
                .ToList();
        }
        
        /// <summary>
        /// General Check if a given coordinate is valid
        /// </summary>
        /// <param name="board"></param>
        /// <param name="coords"></param>
        /// <param name="sideCorner"></param>
        /// <param name="ignoreOwnPiece">Ignores the check that you can't take over your own pieces</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Function will trigger when updateBlockingPieces is true and given piece targeted an enemy
        /// Will check if piece can attack king when targeting piece moves away
        /// </summary>
        /// <param name="board"></param>
        /// <param name="blockingPiece"></param>
        /// <param name="op"></param>
        /// <param name="corner"></param>
        /// <param name="rotation"></param>
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

        /// <summary>
        /// Handles targeting king for pieces that don't have line of sight movements
        /// </summary>
        /// <param name="board"></param>
        /// <param name="coordinate"></param>
        protected void HandleTargetKing(Tile[] board, Coords coordinate)
        {
            if (!board.HasPawnOnTile(coordinate) || board[(int) coordinate]?.Piece.EPiece != EPiece.King)
                return;
            
            ((King) board[(int) coordinate].Piece).Checks.Add(new AttackCoord(Coords, true));
        }
        
        /// <summary>
        /// Handles targeting king for pieces that have line of sight movement
        /// </summary>
        /// <param name="board"></param>
        /// <param name="op"></param>
        /// <param name="corner"></param>
        /// <param name="rotation"></param>
        protected void HandleTargetKing(Tile[] board, Func<int, int, int> op, int corner, int rotation)
        {
            if (board[op((int) Coords, rotation)].Piece.EPiece != EPiece.King)
                return;

            ((King) board[op((int) Coords, rotation)].Piece).Checks.AddRange(GetAttackLine(board, op, rotation, corner));
        }
        
        /// <summary>
        /// Calculates the attack line of sight to the target
        /// Ghost targets are still able to attack when moved but can't be blocked by other pieces
        /// Attack line includes attacking pieces coords
        /// </summary>
        /// <param name="board"></param>
        /// <param name="op"></param>
        /// <param name="rotation"></param>
        /// <param name="corner"></param>
        /// <returns></returns>
        private List<AttackCoord> GetAttackLine(Tile[] board, Func<int, int, int> op, int rotation, int corner)
        {
            List<AttackCoord> attackList = new List<AttackCoord>();

            bool finished = false;
            bool outOfBounds = false;
            bool isAttackBlockable = false;
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
                        isAttackBlockable = true;
                    
                    if(board[(int) coordinate].Piece == this)
                        finished = true;
                }
                    

                attackList.Add(new AttackCoord(coordinate, isAttackBlockable));
                rotation = outOfBounds ? rotation - 1 : rotation + 1;
            }

            return attackList.Distinct().ToList();
        }

        /// <summary>
        /// Updates Coordinates of moved piece
        /// </summary>
        /// <param name="coords"></param>
        internal virtual void Move(Coords coords)
        {
            Coords = coords;
        }
    }
}