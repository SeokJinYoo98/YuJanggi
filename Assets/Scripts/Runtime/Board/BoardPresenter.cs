using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System.Collections.Generic;
    using Yujanggi.Core.Domain;
    using Yujanggi.Runtime.Piece;
    public interface IReplayBoardRenderer
    {
        public void RestoreCapturedPiece(int id, PlayerTeam team, Pos to);
        public void PlaceCapturedPiece(int id, PlayerTeam team);
        public void MovePiece(int id, Pos to);
        public void UnHighlight();
        public void HighlightOnlyPiece(int id);
    }

    public class BoardPresenter : MonoBehaviour, IReplayBoardRenderer
    {
        [SerializeField] private BoardHighlighter _highlighter;
        [SerializeField] private PieceManager _pieces;

        private bool    _isHighlighted = false;
        private int     _deathCnt      = 0;
        private Vector3 _deathPos      = new Vector3(4, 0, -2);

        public void UnHighlightPiece()
            => _pieces.UnHighlightPiece();
        public void HighlightPiece(int pieceId)
            => _pieces.HighlightPiece(pieceId);
        public void HighlightWays(IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals)
        {
            _highlighter.ShowHighlight(legals, true);
            _highlighter.ShowHighlight(illegals, false);
        }
        public void UnHighlightWays()
            => _highlighter.HideHighlight();

        public void StartGame(IBoardModel model)
        {
            _pieces.SpawnPieces(model);
        }

        private void Awake()
        {
           
        }

        public void SetDeathPosition(Vector3 pos)
            => _deathPos = pos;

        public void  RestoreCapturedPiece(int id, PlayerTeam team, Pos to)
        {
            // ref var garbagePos = ref GetGarbagePos(team);
            // garbagePos += Pos.Left;
            --_deathCnt;
            _pieces.DoMove(id, to);
        }
        public void  PlaceCapturedPiece(int id, PlayerTeam team)
        {
            var deathPos = new Vector3(_deathPos.x, _deathPos.y + _deathCnt * 0.1f, _deathPos.z);
            ++_deathCnt;
            _pieces.DoMove(id, deathPos);
        }
        public void  MovePiece(int id, Pos to)
        {
            _pieces.DoMove(id, to);
        }
        public void  UnHighlight()
        {
            if (!_isHighlighted) return;

            _pieces.UnHighlightPiece();
            _highlighter.HideHighlight();
            _isHighlighted = false;
        }
        public void  Highlight(int id, IReadOnlyList<Pos> legalWays, IReadOnlyList<Pos> illegalWays)
        {
            if (_isHighlighted) UnHighlight();
            _pieces.HighlightPiece(id);
            _highlighter.ShowHighlight(legalWays, true);
            _highlighter.ShowHighlight(illegalWays, false);
            _isHighlighted = true;
        }
        public void  ResetGame(IBoardModel model)
        {
            UnHighlight();
            _pieces.ResetViews(model);
            _deathPos = new Vector3(4, 0, -2);
            _deathCnt = 0;
        }
        public void SyncBoardState(IBoardModel boardModel)
        {
            _pieces.ResetViews(boardModel);
        }
        public void HighlightOnlyPiece(int id)
        {
            _pieces.HighlightPiece(id); 
            _isHighlighted = true;
        }
    }
}
