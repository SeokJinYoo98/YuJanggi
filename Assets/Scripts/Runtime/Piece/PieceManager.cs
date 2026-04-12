
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
        public void ResetViews(IBoardModel boardModel)
        {
            int width = boardModel.WIDTH;
            int height = boardModel.HEIGHT;

            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < height; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!boardModel.HasPiece(pos))
                        continue;

                    var pieceInfo = boardModel.GetPiece(pos);
                    _views[pieceInfo.Id].MoveTo(new Pos(x, z));
                }
            }
        }
        public void SpawnPieces(IBoardModel boardModel)
        {
            int width  = boardModel.WIDTH;
            int height = boardModel.HEIGHT;

            for (int x = 0; x < width; ++ x)
            {
                for (int z = 0; z < height; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!boardModel.HasPiece(pos))
                        continue;

                    var pieceInfo = boardModel.GetPiece(pos);
                    var piece = _pieceSpawner.SpawnPiece(pieceInfo, pos);
                    _views[pieceInfo.Id] = piece;
                }
            }
        }
        public void DoMove(int id, Pos to)
        {
            _views[id].MoveTo(to);
        }
    }
}