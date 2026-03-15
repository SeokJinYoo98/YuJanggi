using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Board
{
    public interface IBoardState
    {
        public PlayerType BottomPlayer { get; }
        
       
        public void ClearMovable();
       
       
       
        
 

        //
       
        public int WIDTH { get; }
        public int HEIGHT { get; }
        public bool TryGetPiece(Pos pos, out PieceInfo piece);
        public bool IsTherePiece(Pos pos, out PieceInfo piece);

        public bool IsMovable(Pos pos); 
        public void AddMovable(List<Pos> ways);
        public IReadOnlyList<Pos> MovableCells { get; }

        public bool BoundaryCheck(Pos pos);
        public bool IsPalace(Pos pos);
    }

    public class BoardState : IBoardState
    {
        public int WIDTH  => _width;
        public int HEIGHT => _height;
        private readonly int         _width  = 9;
        private readonly int         _height = 10;
        private CellData[,] _board;
        public BoardState(PlayerType bottom = PlayerType.Cho, int width = 9, int height = 10)
        {
            _bottom = bottom;
            _width = width; _height = height;
            CreateBoard();
            RegisterPiece(bottom);
        }
    

        private PlayerType _bottom;
        public PlayerType   BottomPlayer => _bottom;
        public bool    TryGetPiece(Pos pos, out PieceInfo pieceInfo)
        {
            var x = pos.X; var z = pos.Z;

            pieceInfo = _board[x, z].Piece;
            if (!BoundaryCheck(pos)) 
                return false;

            return true;
        }
        public void MovePiece(Pos fromPos, Pos toPos, out PieceInfo killed)
        {
            var fromX = fromPos.X; var fromZ = fromPos.Z;
            var toX = toPos.X; var toZ = toPos.Z;

            killed = _board[toX, toZ].Piece;
            _board[toX, toZ].Piece = _board[fromX, fromZ].Piece;
            _board[fromX, fromZ].Piece = PieceInfo.None;
        }

        public void SetPiece(PieceInfo piece, int x, int z)
            => _board[x, z].Piece = piece;
        public void RemovePiece(int x, int z)
            => _board[x, z].Piece = new();
        public bool BoundaryCheck(Pos pos)
        {
            return 0 <= pos.X && pos.X < _width && 0 <= pos.Z && pos.Z < _height;
        }
        
        public void ClearMovable()
            => _movableCells.Clear();

       
        public bool IsPalace(Pos pos)
        {
            if (!BoundaryCheck(pos)) 
                return false;
            
            return _board[pos.X, pos.Z].Palace;
        }
        public void AddMovable(List<Pos> ways)
    => _movableCells.AddRange(ways);
        public bool IsMovable(Pos pos)
            => _movableCells.Contains(pos);
        private List<Pos> _movableCells = new(25);
        public IReadOnlyList<Pos> MovableCells => _movableCells;
        // 
        public bool IsTherePiece(Pos pos, out PieceInfo piece)
        {
            piece = _board[pos.X, pos.Z].Piece;

            if (piece.Team == PlayerType.None)
                return false;

            return true;
        }
        private void CreateBoard()
        {
            _board = new CellData[_width, _height];
            for (int x = 0; x < _width; ++x)
            {
                for (int z = 0; z < _height; ++z)
                {
                    _board[x, z] = new();
                }
            }


            for (int x = 3; x <= 5; ++x)
            {
                for (int z = 0; z <= 2; ++z)
                {
                    _board[x, z].Palace = true;
                    _board[x, z + 7].Palace = true;
                }
            }
        }
        private void RegisterPiece(PlayerType bottomPlayer)
        {
            var topPlayer = bottomPlayer == PlayerType.Cho ? PlayerType.Han : PlayerType.Cho;
            SpawnBottom(bottomPlayer);
            SpawnTop(topPlayer);
        }
        private void SpawnBottom(PlayerType bottom)
        {
            Spawn(bottom, PieceType.Chariot, 0, 0);
            Spawn(bottom, PieceType.Chariot, 8, 0);

            Spawn(bottom, PieceType.Elephant, 1, 0);
            Spawn(bottom, PieceType.Elephant, 7, 0);

            Spawn(bottom, PieceType.Horse, 6, 0);
            Spawn(bottom, PieceType.Horse, 2, 0);

            Spawn(bottom, PieceType.Guard, 3, 0);
            Spawn(bottom, PieceType.Guard, 5, 0);

            Spawn(bottom, PieceType.King, 4, 1);

            Spawn(bottom, PieceType.Cannon, 1, 2);
            Spawn(bottom, PieceType.Cannon, 7, 2);

            Spawn(bottom, PieceType.Soldier, 0, 3);
            Spawn(bottom, PieceType.Soldier, 2, 3);
            Spawn(bottom, PieceType.Soldier, 4, 3);
            Spawn(bottom, PieceType.Soldier, 6, 3);
            Spawn(bottom, PieceType.Soldier, 8, 3);
        }
        private void SpawnTop(PlayerType top)
        {
            Spawn(top, PieceType.Chariot, 0, 9);
            Spawn(top, PieceType.Chariot, 8, 9);

            Spawn(top, PieceType.Elephant, 1, 9);
            Spawn(top, PieceType.Elephant, 7, 9);

            Spawn(top, PieceType.Horse, 6, 9);
            Spawn(top, PieceType.Horse, 2, 9);

            Spawn(top, PieceType.Guard, 3, 9);
            Spawn(top, PieceType.Guard, 5, 9);

            Spawn(top, PieceType.King, 4, 8);

            Spawn(top, PieceType.Cannon, 1, 7);
            Spawn(top, PieceType.Cannon, 7, 7);

            Spawn(top, PieceType.Soldier, 0, 6);
            Spawn(top, PieceType.Soldier, 2, 6);
            Spawn(top, PieceType.Soldier, 4, 6);
            Spawn(top, PieceType.Soldier, 6, 6);
            Spawn(top, PieceType.Soldier, 8, 6);

        }
        private void Spawn(PlayerType team, PieceType type, int x, int z)
        {
            _board[x, z].Piece.Team = team;
            _board[x, z].Piece.Type = type;
        }
    }
}
