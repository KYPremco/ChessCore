using System;
using System.Collections.Generic;
using System.Linq;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Events;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game
{
    public class GameManager : GameEvents
    {
        public Player Turn { get; private set; }

        public Board.Board Board { get; }

        public GameManager()
        {
            Board = new Board.Board();
            Turn = Player.White;
        }
        
        public void Start()
        {
            LoadTiles();
            UpdateBlockingPieces(Board.WhitePieces, Board.BlackPieces);
            UpdateBlockingPieces(Board.BlackPieces, Board.WhitePieces);
            OnLoaded(Board.Tiles);
        }

        /// <summary>
        /// Load the game board in with the given starting pieces
        /// </summary>
        private void LoadTiles()
        {
            for (int i = 0; i < 64; i++)
            {
                Board.Tiles[i] = new Tile((Coords) i);
                if (!Board.Tiles[i].Coords.HasStartingPiece())
                    continue;

                Piece piece = Board.Tiles[i].Coords.GetStartingPiece();
                Board.Tiles[i].Piece = piece;
                AddPieceToBoard(piece);
            }
        }

        /// <summary>
        /// Adds pieces to the correct board list
        /// </summary>
        /// <param name="piece"></param>
        private void AddPieceToBoard(Piece piece)
        {
            if (piece.Side == Player.White)
            {
                if (piece.EPiece == EPiece.King)
                {
                    Board.WhiteKing = (King) piece;
                }
                else
                {
                    Board.WhitePieces.Add(piece);
                }
            }
            else
            {
                if (piece.EPiece == EPiece.King)
                {
                    Board.BlackKing = (King) piece;
                }
                else
                {
                    Board.BlackPieces.Add(piece);
                }
            }
        }

        /// <summary>
        /// Updates the game cycle which player can move and refresh the state of the board
        /// </summary>
        private void UpdateGameCycle()
        {
            SwitchPlayer();

            Board.BlackKing.Checks.Clear();
            Board.WhiteKing.Checks.Clear();
            UpdateBlockingPieces(Board.WhitePieces, Board.BlackPieces);
            UpdateBlockingPieces(Board.BlackPieces, Board.WhitePieces);
        }

        /// <summary>
        /// Pre-compute the pieces that are keeping the king safe
        /// </summary>
        /// <param name="friendlyPieces"></param>
        /// <param name="enemyPieces"></param>
        private void UpdateBlockingPieces(List<Piece> friendlyPieces, List<Piece> enemyPieces)
        {
            foreach (Piece piece in friendlyPieces)
            {
                piece.Blocking.Clear();
            }

            foreach (Piece piece in enemyPieces)
            {
                piece.UpdateAvailableCoords(Board.Tiles);
            }
        }

        /// <summary>
        /// Get available coordinates from given piece making sure that all chess rules are applied
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="xRay"></param>
        /// <returns></returns>
        public List<Coords> GetAvailableCoords(Piece piece, bool xRay = false)
        {
            if (piece.EPiece == EPiece.King)
                return GetKingAvailableCoords((King) piece);

            King king = piece.Side == Player.White ? Board.WhiteKing : Board.BlackKing;
            if (king.Checks.Count == 0)
                return piece.AvailableCoords(Board.Tiles, xRay);

            return piece.AvailableCoords(Board.Tiles, xRay)
                .Intersect(king.Checks.Where(x => !x.IsGhost).Select(x => x.Coordinate))
                .ToList();
        }

        /// <summary>
        /// Gets all coordinates from a given player, may ignore king coordinates to prevent looping
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ignoreKing"></param>
        /// <param name="xRay"></param>
        /// <returns></returns>
        public List<Coords> GetAllAvailableCoords(Player player, bool ignoreKing = false, bool xRay = false)
        {
            List<Coords> availableCoords =
                GetAllAvailableCoords(player == Player.White ? Board.WhitePieces : Board.BlackPieces, xRay)
                    .Distinct()
                    .ToList();

            if (ignoreKing)
                return availableCoords;

            return availableCoords
                .Concat(GetKingAvailableCoords(player == Player.White ? Board.WhiteKing : Board.BlackKing))
                .ToList();
        }

        /// <summary>
        /// Gets all available coordinates from given list
        /// </summary>
        /// <param name="pieceList"></param>
        /// <param name="xRay"></param>
        /// <returns></returns>
        private List<Coords> GetAllAvailableCoords(List<Piece> pieceList, bool xRay)
        {
            return pieceList
                .SelectMany(piece => GetAvailableCoords(piece, xRay))
                .ToList();
        }

        /// <summary>
        /// Coordinates king can move to
        /// </summary>
        /// <param name="king"></param>
        /// <returns></returns>
        private List<Coords> GetKingAvailableCoords(King king)
        {
            return king
                .AvailableCoords(Board.Tiles)
                .Except(GetAllAvailableCoords(king.Side == Player.White ? Player.Black : Player.White, true, true))
                .Except(king.Side == Player.White
                    ? Board.BlackKing.AvailableCoords(Board.Tiles)
                    : Board.WhiteKing.AvailableCoords(Board.Tiles))
                .Except(king.Checks.Where(x => x.IsGhost).Select(x => x.Coordinate))
                .Concat(GetKingCastlingCoords(king))
                .ToList();
        }

        /// <summary>
        /// Unique rule checking for king castling 
        /// </summary>
        /// <param name="king"></param>
        /// <returns>returns king positions when castling is possible</returns>
        private List<Coords> GetKingCastlingCoords(King king)
        {
            List<Coords> castling = new List<Coords>();

            if (king.HasMoved || king.Checks.Count > 0)
                return castling;
            
            List<Coords> availableAttacks = GetAllAvailableCoords(king.Side == Player.White ? Player.Black : Player.White, true);
            King enemyKing = king.Side == Player.White ? Board.BlackKing : Board.WhiteKing;
            
            if (!availableAttacks.Contains(king.Coords + 2) 
                && !enemyKing.AvailableCoords(Board.Tiles).Contains(king.Coords + 2)
                && !availableAttacks.Contains(king.Coords + 1)
                && Board.Tiles[(int)king.Coords + 4].Piece?.EPiece == EPiece.Rook
                && !((Rook)Board.Tiles[(int)king.Coords + 4].Piece).HasMoved)
                castling.Add(king.Coords + 2);
            
            if (!availableAttacks.Contains(king.Coords - 2) 
                && !enemyKing.AvailableCoords(Board.Tiles).Contains(king.Coords - 2)
                && !availableAttacks.Contains(king.Coords - 1)
                && Board.Tiles[(int)king.Coords - 3].Piece?.EPiece == EPiece.Rook
                && !((Rook)Board.Tiles[(int)king.Coords - 3].Piece).HasMoved)
                castling.Add(king.Coords - 2);

            return castling;
        }

        /// <summary>
        /// Moves pieces if possible, activates gameCycle
        /// </summary>
        /// <param name="fromCoords"></param>
        /// <param name="newCoords"></param>
        /// <returns>Returns true when piece was moved</returns>
        public void MovePiece(Coords fromCoords, Coords newCoords)
        {
            Piece piece = Board.Tiles[(int) fromCoords].Piece;
            if (piece.Side != Turn || !GetAvailableCoords(piece).Contains(newCoords))
                return;

            if (MoveCastling(fromCoords, newCoords))
                return;
            
            if (Board.Tiles.HasPawnOnTile(newCoords))
                TakeOverPiece(Board.Tiles[(int) newCoords].Piece);

            piece.Move(newCoords);
            Board.Tiles[(int) fromCoords].Piece = null;
            Board.Tiles[(int) newCoords].Piece = piece;

            UpdateGameCycle();
            OnPieceMoved(piece, fromCoords, newCoords);
        }
        
        /// <summary>
        /// Unique move for castling
        /// </summary>
        /// <param name="fromCoords"></param>
        /// <param name="newCoords"></param>
        /// <returns>returns false if castling was not activated</returns>
        private bool MoveCastling(Coords fromCoords, Coords newCoords)
        {
            Tile kingTile = Board.Tiles[(int) fromCoords];

            if (kingTile.Piece.EPiece != EPiece.King || ((King)kingTile.Piece).HasMoved)
                return false;
            
            int fixRookCoords = newCoords.Column() == 5 ? 2 : -1;
            Tile rookTile = Board.Tiles[(int) newCoords + fixRookCoords];

            if (kingTile.Piece?.EPiece != EPiece.King || rookTile.Piece?.EPiece != EPiece.Rook || kingTile.Piece?.Side != rookTile.Piece?.Side)
                return false;
            
            if (rookTile.Coords.Column() == 7)
            {
                //Rook
                Board.Tiles[(int) rookTile.Coords - 3].Piece = rookTile.Piece;
                rookTile.Piece.Move(rookTile.Coords - 3);
                OnPieceMoved(rookTile.Piece, rookTile.Coords, rookTile.Coords - 3);
                
                //King
                Board.Tiles[(int) kingTile.Coords + 2].Piece = kingTile.Piece;
                kingTile.Piece.Move(kingTile.Coords + 2);
                OnPieceMoved(kingTile.Piece, kingTile.Coords, rookTile.Coords + 2);
            }
            else
            {
                //Rook
                Board.Tiles[(int) rookTile.Coords + 2].Piece = rookTile.Piece;
                rookTile.Piece.Move(rookTile.Coords + 2);
                OnPieceMoved(rookTile.Piece, rookTile.Coords, rookTile.Coords + 2);
                
                //King
                Board.Tiles[(int) kingTile.Coords - 2].Piece = kingTile.Piece;
                kingTile.Piece.Move(kingTile.Coords - 2);
                OnPieceMoved(kingTile.Piece, kingTile.Coords, kingTile.Coords - 2);
            }
            Board.Tiles[(int) fromCoords].Piece = null;
            Board.Tiles[(int) newCoords + fixRookCoords].Piece = null;
            
            UpdateGameCycle();
            return true;
        }

        /// <summary>
        /// Moves the taken over piece to the correct list.
        /// </summary>
        /// <param name="piece"></param>
        private void TakeOverPiece(Piece piece)
        {
            if (piece.Side == Player.White)
            {
                Board.WhitePieces.Remove(piece);
            }
            else
            {
                Board.BlackPieces.Remove(piece);
            }
            OnPieceTakenOver(piece, piece.Coords);
        }

        /// <summary>
        /// Swaps player that can move a piece
        /// </summary>
        private void SwitchPlayer()
        {
            Turn = Turn == Player.White ? Player.Black : Player.White;
        }
    }
}