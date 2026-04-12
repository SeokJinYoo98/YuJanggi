using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System.Collections.Generic;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Match;
    using Yujanggi.Runtime.Piece;
    public interface IBoardPresenter
    {

    }
    public class BoardPresenter : MonoBehaviour, IBoardPresenter
    {
        private BoardView _boardView;
        [SerializeField] private PieceManager _pieces;

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
            garbagePos += Pos.Left;
            _pieces.DoMove(id, to);
        }
        public void  PlaceCapturedPiece(int id, PlayerTeam team)
        {
            ref var garbagePos = ref GetGarbagePos(team);
            var to = garbagePos;
            garbagePos += Pos.Right;
            _pieces.DoMove(id, to);
        }
        public void  MovePiece(int id, Pos to)
        {
            _pieces.DoMove(id, to);
        }
        public void  UnHighlight()
        {
            _pieces.UnHighlight();
            _boardView.UnHighlight();
        }
        public void  Highlight(int id, IReadOnlyList<Pos> legalWays, IReadOnlyList<Pos> illegalWays)
        {
            _pieces.HighlightPiece(id);
            _boardView.Highlight(legalWays, illegalWays);
        }
        public void  ResetGame(IBoardModel model)
        {
            UnHighlight();
            _pieces.ResetViews(model);
            _garbageChoPos = new Pos(0, -1);
            _garbagehanPos = new Pos(0, -2);
        }
    }
}
