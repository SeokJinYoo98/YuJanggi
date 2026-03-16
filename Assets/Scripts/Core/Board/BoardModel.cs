using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.Board;

namespace Yujanggi.Core.Board
{
    public interface IBoardModel
    {
        public int WIDTH { get; }
        public int HEIGHT { get; }
        public bool         IsInside(Pos pos);
        public bool         IsPalace(Pos pos);
        public bool         HasPiece(Pos pos);
        public PieceInfo    GetPiece(Pos pos);
        public void         SetPiece(Pos pos, PieceInfo piece);
    }

    public class BoardModel : IBoardModel
    {
        public int WIDTH  => _width;
        public int HEIGHT => _height;

        private readonly int _width  = 9;
        private readonly int _height = 10;

        private CellData[,]  _board;

        public BoardModel(int width = 9, int height = 10)
        {
            _width = width; _height = height;
            CreateBoard();
        }
        // 생성 관련
        private void CreateBoard()
        {
            _board = new CellData[_width, _height];
            for (int x = 0; x < _width; ++x)
            {
                for (int z = 0; z < _height; ++z)
                {
                    _board[x, z] = new();
                    _board[x, z].Piece = PieceInfo.None;
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

        // 공개 API
        public bool         IsPalace(Pos pos)
            => _board[pos.X, pos.Z].Palace;
        public bool         IsInside(Pos pos)
            => 0 <= pos.X && pos.X < _width && 0 <= pos.Z && pos.Z < _height;
        public bool         HasPiece(Pos pos)
            => _board[pos.X, pos.Z].Piece != PieceInfo.None;
        public PieceInfo    GetPiece(Pos pos)
            => _board[pos.X, pos.Z].Piece;
        public void         SetPiece(Pos pos, PieceInfo piece)
            => _board[pos.X, pos.Z].Piece = piece;
    }
}
