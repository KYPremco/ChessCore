using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Board
{
    public class Tile
    {
        private readonly Coords _coords;

        private Piece _piece;

        public Coords Coords => _coords;

        public Tile(Coords coords)
        {
            _coords = coords;
        }

        public Piece Piece
        {
            get => _piece;
            set => _piece = value;
        }
    }
}