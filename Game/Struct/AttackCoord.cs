using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Struct
{
    public readonly struct AttackCoord
    {
        /// <summary>
        /// Coordinate of attack
        /// </summary>
        public Coords Coordinate { get; }
        
        /// <summary>
        /// Placing another piece on a ghost doesn't block trajectory 
        /// </summary>
        public bool IsAttackBlockable { get; }
        
        public AttackCoord(Coords coordinate, bool isAttackBlockable)
        {
            Coordinate = coordinate;
            IsAttackBlockable = isAttackBlockable;
        }
    }
}