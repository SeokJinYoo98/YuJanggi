using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Board
{
    using Yujanggi.Runtime.Piece;
   
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private BoardHighlighter  _highlighter;

        public void Highlight(in IReadOnlyList<Pos> movablePositions)
        {
            _highlighter.ShowHighlight(movablePositions);
        }
        public void UnHighlight()
        {
            _highlighter.HideHighlight();
        }
    }
}