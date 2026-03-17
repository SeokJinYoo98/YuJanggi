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

        private IPiece[,] _mapArr;
        private IPiece _highlightedPiece;

        private IPiece this[Pos p]
        {
            get => _mapArr[p.X, p.Z];
            set => _mapArr[p.X, p.Z] = value;
        }
        public void DoMove(Pos fromPos, Pos toPos, out IPiece killed)
        {
            var piece   = this[fromPos];
            if (piece == null)
            {
                killed = null;
                return;
            }
            killed = this[toPos];
            piece.MoveTo(toPos);
            this[fromPos] = null;
            this[toPos] = piece;
        }
        public void ShowHighlights(Pos selectedPos, IReadOnlyList<Pos> movablePositions)
        {
            if (_highlightedPiece != null)
                HideHighlights();

            _highlightedPiece = this[selectedPos];
            _highlightedPiece.Highlight();
            _highlighter.ShowHighlight(movablePositions);
        }
        public void HideHighlights()
        {
            _highlightedPiece.Highlight();
            _highlightedPiece = null;
            _highlighter.HideHighlight();
        }
        public void SpawnPieceView(IBoardModel board, PlayerTeam bottom)
        {
            int width = board.WIDTH;
            int height = board.HEIGHT;
            _mapArr = new Piece[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var pos = new Pos(i, j);
                    if (!board.HasPiece(pos))
                    {
                        this[pos] = null;
                        continue;
                    }
                    var pieceInfo = board.GetPiece(pos);
                    var piece = _pieceSpawner.Spawn(pieceInfo, pos, bottom);
                    this[pos] = piece;
                }
            }

        }

        public void UndoMove(in MoveContext context)
        {
            var from = context.Record.From;
            var to = context.Record.To;

            var moved = this[to];
            if (moved == null) return;

            this[to] = null;

            moved.MoveTo(from);
            this[from] = moved;

            if (context.IsCapture)
            {
                var captured = context.CapturedPieceView;
                this[to] = captured;
                captured?.MoveTo(to);
            }
        }
    }
}