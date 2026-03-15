using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;

    // ToDo: 보드인포 IPiece 제거.   
    // 보드 컨트롤러가 될 예정
    public class BoardController : MonoBehaviour
    {
        private BoardView   _boardView;
        private BoardState  _boardState;
        private JanggiRule  _janggiRule;

        private BoardInfo   _boardInfo;

        private void Awake()
        {
            _boardView = GetComponent<BoardView>();
            _janggiRule = new();
        }
        public void StartGame(PlayerType bottomPlayer)
        {
            _boardInfo   = new(bottomPlayer);
            _boardState  = new(bottomPlayer);
            _boardView.SpawnPieceView(_boardState);
        }
        public bool SelectPiece(Pos pos, PlayerType team)
        {
            if (!TrySelectPiece(team, pos))
                return false;
            FindWays();
            RefreshSelectionView();
            return true;
        }
        private bool TrySelectPiece(PlayerType team, Pos pos)
        {
            if (!_boardState.TryGetPiece(pos, out var pieceInfo))
                return false;

            if (pieceInfo.Team != team)
                return false;
            _boardInfo.Select(pieceInfo, pos);
            return true;
        }
        private void FindWays()
        {
            _janggiRule.FindWays(_boardState, _boardInfo);
        }
        public bool SelectWay(Pos pos)
        {
            if (!_boardState.IsMovable(pos))
            {
                UnSelect();
                return false;
            }
            MovePiece(pos);
            UnSelect();
            return true;
        }
        private void RefreshSelectionView()
        {
            _boardView.HighlightBoard(_boardState, _boardInfo);
        }
        private void UnSelect()
        {
            _boardState.ClearMovable(); 
            RefreshSelectionView(); 
            _boardInfo.Clear();
        }
        private void MovePiece(Pos toPos)
        {
            var fromPos = _boardInfo.Pos;
            _boardState.MovePiece(fromPos, toPos, out  _);
            _boardView.ApplyMoveView(fromPos, toPos, out  _);
        }
    }
}
