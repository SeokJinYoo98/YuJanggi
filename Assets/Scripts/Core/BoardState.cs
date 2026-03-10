using NUnit.Framework;
using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
using UnityEngine;
using Yujanggi.Core.Movement;
namespace Yujanggi.Core.Board
{
    public interface IBoardState
    {
        public bool BoundaryCheck(int x, int z);
        public bool IsTherePiece(int x, int z, out PlayerType type);
        public void ClearMovable();
        public void AddMovable(int x, int z);
        public bool IsMovable(int x, int z);
        public IReadOnlyList<(int x, int z)> MovableCells { get; }

    }
    public class BoardState : IBoardState
    {
        private int         WIDTH  = 9;
        private int         HEIGHT = 10;
        private CellData[,] _board;
 
        public BoardState(int width = 9, int height = 10)
        {
            WIDTH = width; HEIGHT = height;
            _board = new CellData[HEIGHT, WIDTH];

            for (int z = 0; z < HEIGHT; ++z)
            {
                for (int x = 0; x < WIDTH; ++x)
                {
                    _board[z, x] = new CellData();
                }
            }

            for (int x = 3; x <= 5; ++x)
            {
                for (int z = 0; z <= 2; ++z)
                {
                    _board[z, x].Palace = true;
                    _board[z + 7, x].Palace = true;
                }
            }
        }
        public IPiece       GetPiece(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x].Piece;
        }
        public MovementInfo MovePiece(int fromX, int formZ, int toX, int toZ)
        {
            _board[formZ, fromX].Piece?.MoveTo(toX, toZ);
            _board[toZ, toX].Piece = _board[formZ, fromX].Piece;
            _board[formZ, fromX].Piece = null;

            return new();
        }
        public CellData     GetCell(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x];
        }
        public void         SetPiece(IPiece piece, int x, int z)
            => _board[z, x].Piece = piece;
        public void         RemovePiece(int x, int z)
            => _board[z, x] = null;
        public bool         BoundaryCheck(int x, int z)
            => 0 <= x && x < WIDTH && 0 <= z && z < HEIGHT;
        public bool         IsTherePiece(int x, int z, out PlayerType type)
        {
            var p = GetPiece(x, z);
            if (p == null)
            {
                type = PlayerType.None;
                return false;
            }
            type = p.Team;
            return true;
        }


        private List<(int x, int z)> _movableCells = new();
        public void ClearMovable()
            => _movableCells.Clear();
        public void AddMovable(int x, int z)
            => _movableCells.Add((x, z));
        public bool IsMovable(int x, int z)
            => _movableCells.Contains((x, z));
        public IReadOnlyList<(int x, int z)> MovableCells => _movableCells;
    }
}
