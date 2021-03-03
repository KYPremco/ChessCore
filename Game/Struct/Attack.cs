using OnlineChessCore.Game.Board;

namespace OnlineChessCore.Game.Struct
{
    public readonly struct Attack
    {
        public Coords Coordinate { get; }
        public bool IsGhost { get; }
        
        public Attack(Coords coordinate, bool isGhost)
        {
            Coordinate = coordinate;
            IsGhost = isGhost;
        }
    }
}