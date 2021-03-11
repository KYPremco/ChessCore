using System;
using OnlineChessCore.Game.Board;
using OnlineChessCore.Game.Events.Args;
using OnlineChessCore.Game.Pieces;

namespace OnlineChessCore.Game.Events
{
    public class GameEvents
    {
        public event EventHandler<PieceTakenOverArgs> PieceTakenOver;
        
        protected virtual void OnPieceTakenOver(Piece takenPiece, Coords coords)
        {
            PieceTakenOver?.Invoke(this, new PieceTakenOverArgs() { TakenPiece = takenPiece, TakenCoords = coords });
        }

        public event EventHandler<PieceMovedArgs> PieceMoved;
        
        protected virtual void OnPieceMoved(Piece piece, Coords fromCoords, Coords newCoords)
        {
            PieceMoved?.Invoke(this, new PieceMovedArgs() { MovedPiece = piece, FromCoords = fromCoords, NewCoords = newCoords });
        }

        public event EventHandler PawnUpgrading;
        
        protected virtual void OnPawnUpgrading()
        {
            PawnUpgrading?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<PawnUpgradedArgs> PawnUpgraded;

        protected virtual void OnPawnUpgraded(Coords coords, Piece newPiece)
        {
            PawnUpgraded?.Invoke(this, new PawnUpgradedArgs() { Coords = coords, NewPiece = newPiece });
        }

        public event EventHandler<LoadedArgs> Loaded;

        protected virtual void OnLoaded(Tile[] board)
        {
            Loaded?.Invoke(this, new LoadedArgs() { Board = board });
        }
    }
}