using System;
using System.Collections.Generic;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Struct;

namespace OnlineChessCore.Game.Pieces
{
    public class King : Piece
    {
        public override EPiece EPiece { get; } = EPiece.King;
        
        internal bool HasMoved { get; private set; }

        internal List<Attack> Checks { get; }

        public King(Coords coords, Player player) : base(coords, player)
        {
            Checks = new List<Attack>();
        }
        
        protected override List<Coords> AvailableCoords(Tile[] board, bool xRay, bool updateBlockingPieces)
        {
            List<Coords> coordsList = new List<Coords>();

            coordsList.AddRange(HandleCoords(board, 7, (c) => c + 1));
            coordsList.AddRange(HandleCoords(board, -1, (c) => c + 8));
            coordsList.AddRange(HandleCoords(board, 0, (c) => c - 1));
            coordsList.AddRange(HandleCoords(board, -1, (c) => c - 8));
            coordsList.AddRange(HandleCoords(board, 0, (c) => c + 7));
            coordsList.AddRange(HandleCoords(board, 7, (c) => c + 9));
            coordsList.AddRange(HandleCoords(board, 7, (c) => c - 7));
            coordsList.AddRange(HandleCoords(board, 0, (c) => c - 9));

            return coordsList;
        }
        
        /// <summary>
        /// Reusable method for calculating valid position within the board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="corner"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private IEnumerable<Coords> HandleCoords(Tile[] board, int corner, Func<int, int> op)
        {
            List<Coords> coordsList = new List<Coords>();
            
            if(!ValidCoordinate(board, (Coords) op((int) Coords), corner))
                return coordsList;

            board[op((int) Coords)].IsActive = true;
            coordsList.Add((Coords) op((int) Coords));

            return coordsList;
        }
        
        /// <summary>
        /// King has switched from original place
        /// </summary>
        /// <param name="coords"></param>
        internal override void Move(Coords coords)
        {
            Coords = coords;
            HasMoved = true;
        }
    }
}