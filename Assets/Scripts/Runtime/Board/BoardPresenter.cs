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
        private BoardView _boardView;
        [SerializeField] private PieceManager _pieces;
        private bool _isHighlighted = false;
        private Pos _garbageChoPos = new Pos(0, -1);
        private Pos _garbagehanPos = new Pos(0, -2);

        private void Awake()
        {
            _boardView = GetComponent<BoardView>();   
        }
       
        private ref Pos GetGarbagePos(PlayerTeam team)
        {
            if (team == PlayerTeam.Cho)
                return ref _garbageChoPos;
            return ref _garbagehanPos;
        }
        public void  StartGame(IBoardModel model)
        {
            _pieces.SpawnPieces(model);
        }
        public void  RestoreCapturedPiece(int id, PlayerTeam team, Pos to)
        {
            ref var garbagePos = ref GetGarbagePos(team);
            // garbagePos += Pos.Left;
            _pieces.DoMove(id, to);
        }
        public void  PlaceCapturedPiece(int id, PlayerTeam team)
        {
            ref var garbagePos = ref GetGarbagePos(team);
            var to = garbagePos;
            // garbagePos += Pos.Right;
            _pieces.DoMove(id, to);
        }
        public void  MovePiece(int id, Pos to)
        {
            _pieces.DoMove(id, to);
        }
        public void  UnHighlight()
        {
            if (!_isHighlighted) return;

            _pieces.UnHighlight();
            _boardView.UnHighlight();
            _isHighlighted = false;
        }
        public void  Highlight(int id, IReadOnlyList<Pos> legalWays, IReadOnlyList<Pos> illegalWays)
        {
            if (_isHighlighted) UnHighlight();
            _pieces.HighlightPiece(id);
            _boardView.Highlight(legalWays, illegalWays); 
            _isHighlighted = true;
        }
        public void  ResetGame(IBoardModel model)
        {
            UnHighlight();
            _pieces.ResetViews(model);
            _garbageChoPos = new Pos(0, -1);
            _garbagehanPos = new Pos(0, -2);
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
