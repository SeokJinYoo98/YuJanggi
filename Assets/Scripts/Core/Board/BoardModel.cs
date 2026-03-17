using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.Board;
using static UnityEngine.Audio.ProcessorInstance;

namespace Yujanggi.Core.Board
{
    public interface IBoardModel
    {
        public Pos GetKingPos(PlayerTeam team);
        public int WIDTH { get; }
        public int HEIGHT { get; }

        public bool         IsInside(Pos pos);
        public bool         IsPalace(Pos pos);
        public bool         HasPiece(Pos pos);
        public PieceInfo    GetPiece(Pos pos);
        public void         SetPiece(Pos pos, PieceInfo piece);

        public MoveRecord DoMove(Pos from, Pos to);
        public void UndoMove(MoveRecord moveRecord);
    }

    public class BoardModel : IBoardModel
    {
        public int WIDTH  => _width;
        public int HEIGHT => _height;

        public BoardModel(PlayerTeam bottom, int width = 9, int height = 10)
        {
            _width = width; _height = height;
            CreateBoard();

            if (bottom == PlayerTeam.Cho)
            {
                _choKingPos = new Pos(4, 1);
                _hanKingPos = new Pos(4, 8);
            }
            else
            {
                _hanKingPos = new Pos(4, 1);
                _choKingPos = new Pos(4, 8);
            }

        }

        // 공개 API Set은 공개인가?
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




        public Pos  GetKingPos(PlayerTeam team)
            => team == PlayerTeam.Cho ? _choKingPos : _hanKingPos;

        public MoveRecord DoMove(Pos from, Pos to)
        {
            var moved       = GetPiece(from);
            var captured    = GetPiece(to);

            SetPiece(from, PieceInfo.None);
            SetPiece(to, moved);

            UpdateKingPos(to, moved);

            return new(from, to, moved, captured);
        }
        public void UndoMove(MoveRecord moveRecord)
        {
            var from = moveRecord.From;
            var to = moveRecord.To;

            var moved = moveRecord.MovedPiece;

            SetPiece(from, moved);
            SetPiece(to, moveRecord.IsCapture ? moveRecord.CapturedPiece : PieceInfo.None);

            UpdateKingPos(from, moved);
        }
        // private
        private CellData[,] _board;

        private readonly int _width = 9;
        private readonly int _height = 10;

        private Pos _choKingPos;
        private Pos _hanKingPos;
       
        // private
        private void UpdateKingPos(Pos pos, PieceInfo piece)
        {
            if (piece.Type != PieceType.King)
                return;

            if (piece.Team == PlayerTeam.Cho)
                _choKingPos = pos;
            else
                _hanKingPos = pos;
        }
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
    }
}
