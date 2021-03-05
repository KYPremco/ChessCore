namespace OnlineChessCore.Game.Board
{
    public static class BoardHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool HasPawnOnTile(this Tile[] board, Coords coords)
        {
            return board[(int)coords].Piece != null;
        }
    }
}