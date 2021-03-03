namespace OnlineChessCore.Game.Board
{
    public static class BoardHelper
    {
        public static bool HasPawnOnTile(this Tile[] board, Coords coords)
        {
            return board[(int)coords].Piece != null;
        }
    }
}