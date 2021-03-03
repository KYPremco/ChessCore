using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Board
{
    public class Tile
    {
        private readonly Coords _coords;

        private Piece _piece;

        private bool _isActive;

        public Coords Coords => _coords;

        public Tile(Coords coords)
        {
            _coords = coords;
            _isActive = false;
        }

        public Piece Piece
        {
            get => _piece;
            set => _piece = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
    }
}