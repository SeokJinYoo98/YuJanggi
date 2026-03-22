
using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Piece
{
    public class PieceManager : MonoBehaviour
    {
        [SerializeField] private PieceSpawner _pieceSpawner;
        private readonly Dictionary<int, Piece> _views = new();

        private Pos _garbageChoPos = new Pos(0, -2);
        private Pos _garbagehanPos = new Pos(0, -3);

        private int _currPiece;
        public void UnHighlight()
        {
            if (_currPiece == -1) return;
            _views[_currPiece].UnHighlight();
            _currPiece = -1;
        }
        public void HighlightPiece(int id)
        {
            _currPiece = id;
            _views[_currPiece].Highlight();
        }     
        public void SpawnPieces(IBoardModel boardModel, PlayerTeam bottom)
        {
            int width = boardModel.WIDTH;
            int height = boardModel.HEIGHT;

            for (int x = 0; x < width; ++ x)
            {
                for (int z = 0; z < height; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!boardModel.HasPiece(pos))
                        continue;

                    var pieceInfo = boardModel.GetPiece(pos);
                    var piece = _pieceSpawner.SpawnPiece(pieceInfo, pos, bottom);
                    _views[pieceInfo.Id] = piece;
                }
            }
        }
        public void DoMove(in MoveRecord record)
        {
            var toPos = record.To;
            var movedId = record.MovedPiece.Id;
            _views[movedId].MoveTo(toPos);
            CaptureCheck(record);
        }
        public void UnDoMove(in MoveRecord record)
        {
            var fromPos = record.From;
            var movedId = record.MovedPiece.Id;
            _views[movedId].MoveTo(fromPos);
            UndoCapturedCheck(record);
        }
        private void UndoCapturedCheck(in MoveRecord record)
        {
            if (!record.IsCapture) return;
            var team        = record.CapturedPiece.Team;
            var capturedId  = record.CapturedPiece.Id;
            var toPos = record.To;
            _views[capturedId].MoveTo(toPos);
            if (team == PlayerTeam.Cho)
            {
                _garbageChoPos += Pos.Left;
            }
            else
            {
                _garbagehanPos += Pos.Left;
            }
        }
        private void CaptureCheck(in MoveRecord record)
        {
            if (!record.IsCapture) return;

            var capturedId  = record.CapturedPiece.Id;
            var team        = record.CapturedPiece.Team;

            if (team == PlayerTeam.Cho)
            {
                _views[capturedId].MoveTo(_garbageChoPos);
                _garbageChoPos += Pos.Right;
            }
            else
            {
                _views[capturedId].MoveTo(_garbagehanPos);
                _garbagehanPos += Pos.Right;
            }
        }
    }
}