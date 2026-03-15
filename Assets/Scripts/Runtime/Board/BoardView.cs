using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private PieceSpawner      _pieceSpawner;
        [SerializeField] private BoardHighlighter  _highlighter;

        private Dictionary<Pos, IPiece> _map;
        private void Awake()
        {
            _map = new();
            
        }
        public void ApplyMoveView(Pos fromPos, Pos toPos, out IPiece killed)
        {
            killed = _map[toPos];
            var piece = _map[fromPos];
            if (piece == null) return;
            piece.MoveTo(toPos);
            _map[fromPos] = null;
            _map[toPos] = piece;
            piece.Highlight();
        }
        public void HighlightBoard(IBoardState board, in BoardInfo info)
        {
            HighlightWays(board);
            HighlightPiece(info.Pos);
        }
        private void HighlightWays(IBoardState board)
        {
            var movableCells = board.MovableCells;
            _highlighter.Highlight(movableCells);
        }
        private void HighlightPiece(Pos pos)
        {
            _map[pos]?.Highlight();
        }
        public void SpawnPieceView(IBoardState board)
        {
           
            int width = board.WIDTH;
            int height = board.HEIGHT;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var pos = new Pos(i, j);
                    if (!board.IsTherePiece(pos, out var pieceInfo))
                    {
                        _map[pos] = null;
                        continue;
                    }
                    var piece = _pieceSpawner.Spawn(pieceInfo, pos);
                    _map[pos] = piece;
                }
            }

        }
    }
}