﻿using System.Collections.Generic;
using System.Linq;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game
{
    public class Game
    {
        public Player Turn { get; private set; }
        
        public Board.Board Board { get; }
        
        public Game()
        {
            Board = new Board.Board();
            Turn = Player.White;
            LoadTiles();
            UpdateBlockingPieces(Board.WhitePieces, Board.BlackPieces);
            UpdateBlockingPieces(Board.BlackPieces, Board.WhitePieces);
        }
        
        private void LoadTiles()
        {
            for (int i = 0; i < 64; i++)
            {
                Board.Tiles[i] = new Tile((Coords)i);
                if (!Board.Tiles[i].Coords.HasStartingPiece()) 
                    continue;
                
                Piece piece = Board.Tiles[i].Coords.GetStartingPiece();
                Board.Tiles[i].Piece = piece;
                AddPieceToBoard(piece);
            }
        }
        
        private void AddPieceToBoard(Piece piece)
        {
            if(piece.Side == Player.White)
            {
                if (piece.EPiece == EPiece.King)
                {
                    Board.WhiteKing = (King)piece;
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
                    Board.BlackKing = (King)piece;
                }
                else
                {
                    Board.BlackPieces.Add(piece);
                }
            }
        }
        
        private void UpdateGameCycle()
        {
            SwitchPlayer();
            
            Board.BlackKing.checks.Clear();
            Board.WhiteKing.checks.Clear();
            UpdateBlockingPieces(Board.WhitePieces, Board.BlackPieces);
            UpdateBlockingPieces(Board.BlackPieces, Board.WhitePieces);
        }

        private void UpdateBlockingPieces(List<Piece>  friendlyPieces, List<Piece> enemyPieces)
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
        
        public List<Coords> GetAvailableCoords(Piece piece, bool xRay = false)
        {
            if (piece.EPiece == EPiece.King)
                return GetKingAvailableCoords((King) piece);

            King king = piece.Side == Player.White ? Board.WhiteKing : Board.BlackKing;
            if(king.checks.Count == 0)
                return piece.AvailableCoords(Board.Tiles, xRay);

            return piece.AvailableCoords(Board.Tiles, xRay)
                .Intersect(king.checks.Where(x => !x.IsGhost).Select(x => x.Coordinate))
                .ToList();
        }
        
        public List<Coords> GetAllAvailableCoords(Player player, bool ignoreKing = false, bool xRay = false)
        {
            List<Coords> availableCoords = GetAllAvailableCoords(player == Player.White ? Board.WhitePieces : Board.BlackPieces, xRay)
                .Distinct()
                .ToList(); 

            if (ignoreKing)
                return availableCoords;

            return availableCoords
                .Concat(GetKingAvailableCoords(player == Player.White ? Board.WhiteKing : Board.BlackKing))
                .ToList();
        }
        
        private List<Coords> GetAllAvailableCoords(List<Piece> pieceList, bool xRay)
        {
            return pieceList
                .SelectMany(piece => GetAvailableCoords(piece, xRay))
                .ToList();
        }
        
        private List<Coords> GetKingAvailableCoords(King king)
        {
            return king
                .AvailableCoords(Board.Tiles)
                .Except(GetAllAvailableCoords(king.Side == Player.White ? Player.Black : Player.White, true, true))
                .Except(king.Side == Player.White ? Board.BlackKing.AvailableCoords(Board.Tiles) : Board.WhiteKing.AvailableCoords(Board.Tiles))
                .Except(king.checks.Where(x => x.IsGhost).Select(x => x.Coordinate))
                .ToList();
        }

        public bool MovePiece(Coords oldCoords, Coords newCoords)
        {
            Piece piece = Board.Tiles[(int) oldCoords].Piece;
            if (piece.Side != Turn || !GetAvailableCoords(piece).Contains(newCoords))
                return false;
            
            if(Board.Tiles.HasPawnOnTile(newCoords))
                TakeOverPiece(Board.Tiles[(int)newCoords].Piece);
            
            piece.Move(newCoords);
            Board.Tiles[(int)oldCoords].Piece = null;
            Board.Tiles[(int)newCoords].Piece = piece;
            
            UpdateGameCycle();
            return true;
        }

        private void TakeOverPiece(Piece piece)
        {
            if (piece.Side == Player.White)
            {
                Board.WhitePieces.Remove(piece);
                Board.WhitePiecesTaken.Add(piece);
            }
            else
            {
                Board.BlackPieces.Remove(piece);
                Board.BlackPiecesTaken.Add(piece);
            }
        }
        
        private void SwitchPlayer()
        {
            Turn = Turn == Player.White ? Player.Black : Player.White;
        }
    }
}