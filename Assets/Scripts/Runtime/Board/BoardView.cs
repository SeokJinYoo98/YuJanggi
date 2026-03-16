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

        private IPiece _highlightedPiece;
        private void Awake()
        {
            _map = new();
            
        }
        public void MovePieceView(Pos fromPos, Pos toPos, out IPiece killed)
        {
            killed      = _map[toPos];
            var piece   = _map[fromPos];
            if (piece == null) return;
            piece.MoveTo(toPos);
            _map[fromPos] = null;
            _map[toPos] = piece;
        }
        public void ShowHighlights(Pos selectedPos, IReadOnlyList<Pos> movablePositions)
        {
            if (_highlightedPiece != null)
                HideHighlights();

            _highlightedPiece = _map[selectedPos];
            _highlightedPiece.Highlight();
            _highlighter.ShowHighlight(movablePositions);
        }
        public void HideHighlights()
        {
            _highlightedPiece.Highlight();
            _highlightedPiece = null;
            _highlighter.HideHighlight();
        }

        public void SpawnPieceView(IBoardModel board)
        {
            int width = board.WIDTH;
            int height = board.HEIGHT;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var pos = new Pos(i, j);
                    if (!board.HasPiece(pos))
                    {
                        _map[pos] = null;
                        continue;
                    }
                    var pieceInfo = board.GetPiece(pos);
                    var piece = _pieceSpawner.Spawn(pieceInfo, pos);
                    _map[pos] = piece;
                }
            }

        }
    }
}