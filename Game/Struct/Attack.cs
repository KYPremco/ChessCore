using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Struct
{
    public readonly struct Attack
    {
        /// <summary>
        /// Coordinate of attack
        /// </summary>
        public Coords Coordinate { get; }
        
        /// <summary>
        /// Placing another piece on a ghost doesn't block trajectory 
        /// </summary>
        public bool IsGhost { get; }
        
        public Attack(Coords coordinate, bool isGhost)
        {
            Coordinate = coordinate;
            IsGhost = isGhost;
        }
    }
}