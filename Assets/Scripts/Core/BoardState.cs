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
        public void ClearWays();
        public void AddWay(int x, int z);
        public void PrintWays();
    }
    public class BoardState : IBoardState
    {
        private List<(int x, int z)> _highlightCells;
        public const int WIDTH = 9;
        public const int HEIGHT = 10;

        private CellData[,] _board;

        public BoardState()
        {
            _board = new CellData[HEIGHT, WIDTH];

            for (int z = 0; z < HEIGHT; ++z)
            {
                for (int x = 0; x < WIDTH; ++x)
                {
                    _board[z, x] = new CellData();
                }
            }
            _highlightCells = new();
        }
        public IPiece GetPiece(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x].CurrentPiece;
        }
        public CellData GetCell(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x];
        }
        public CellData this[int x, int z]
        {
            get { return _board[z, x]; }
        }
        public void MoveTo(int fromX, int formZ, int toX, int toZ) 
        {
            HightCell(fromX, formZ);
            _board[formZ, fromX].CurrentPiece?.MoveTo(toX, toZ);
            _board[toZ, toX].CurrentPiece = _board[formZ, fromX].CurrentPiece;
            _board[formZ, fromX].CurrentPiece = null;
        }
       
        
        public void HightCell(int x, int z)
        {
            var piece = _board[z, x].CurrentPiece;
            if (piece != null) piece.Highlight();
        }
        public bool BoundaryCheck(int x, int z)
            => 0 <= x && x < WIDTH && 0 <= z && z < HEIGHT;
        public bool IsTherePiece(int x, int z, out PlayerType type)
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
        public void ClearWays()
            => _highlightCells.Clear();
        public void AddWay(int x, int z)
        {
            _highlightCells.Add((x, z));
        }
        public bool IsValidMovement(int x, int z)
        {
            if (_highlightCells.Contains((x, z))) return true;
            return false;
        }
        public void PrintWays()
        {
            foreach((var x, var z) in _highlightCells)
            {
                Debug.Log($"{x}, {z}");
            }
        }
    }
}
