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

        public void Highlight(IReadOnlyList<Pos> legalWays, IReadOnlyList<Pos> illegalWays)
        {
            _highlighter.ShowHighlight(legalWays, true);
            _highlighter.ShowHighlight(illegalWays, false);
        }
        public void UnHighlight()
        {
            _highlighter.HideHighlight();
        }
    }
}