using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;
    using static UnityEditor.PlayerSettings;

    // ToDo: 보드인포 IPiece 제거.   
    // 보드 컨트롤러가 될 예정
    public class BoardController : MonoBehaviour
    {
        private BoardView   _boardView;
        private BoardState  _boardState;
        private JanggiRule  _janggiRule;

        private SelectionState   _selectionInfo;
       
        private void Awake()
        {
            _boardView = GetComponent<BoardView>();
            _janggiRule = new();
        }
        public void StartGame(PlayerTeam bottomPlayer)
        {
            _selectionInfo   = new(bottomPlayer);
            _boardState      = new(bottomPlayer);
            _boardView.SpawnPieceView(_boardState);
        }
        public BoardActionResult  HandleCellClick(Pos pos, PlayerTeam turn)
        {
            if (!_boardState.IsInside(pos))
                return BoardActionResult.None;

            if (!_selectionInfo.HasSelection)
                return HandleSelectPiece(pos, turn);

            return HandleSelectedClick(pos, turn);
        }
        private BoardActionResult HandleSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!CanSelectPiece(pos, turn))
                return BoardActionResult.SelectFailed;

            SelectPeice(pos);
            FindWays(pos);

            return BoardActionResult.SelectSuccess;
        }
        private BoardActionResult HandleSelectedClick(Pos pos, PlayerTeam turn)
        {
            if (CanSelectPiece(pos, turn))
                ReselectPiece(pos, turn);

            if (!_boardState.IsMovable(pos))
            {
                ClearSelection();
                return BoardActionResult.MoveFailed;
            }


            return MoveSelectedPiece(pos, turn);
        }
        private BoardActionResult ReselectPiece(Pos pos, PlayerTeam turn)
        {
            if (!CanSelectPiece(pos, turn))
            {
                ClearSelection();
                return BoardActionResult.SelectFailed;
            }

            SelectPeice(pos);
            FindWays(pos);
            return BoardActionResult.Reselect;
        }
        private BoardActionResult MoveSelectedPiece(Pos pos, PlayerTeam turn)
        {
            var fromPos = _selectionInfo.SelectedPos;
            _boardState.MovePiece(fromPos, pos, out var killedInfo);
            _boardView.MovePieceView(fromPos, pos, out var killedView);

            ClearSelection();

            if (killedInfo.IsNone)
                return BoardActionResult.MoveSuccess;

            return BoardActionResult.CaptureSuccess;
        }

        private void FindWays(Pos pos)
        {
            _boardState.ClearMovable();
            _janggiRule.FindWays(_boardState, _selectionInfo);
            _boardView.ShowHighlights(pos, _boardState.MovableCells);
        }
        private void ClearSelection()
        {
            _selectionInfo.Clear();
            _boardView.HideHighlights();
        }

        private bool CanSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!_boardState.HasPiece(pos))
                return false;

            var pieceInfo = _boardState.GetPiece(pos);
            return pieceInfo.Team == turn;
        }
        private void SelectPeice(Pos pos)
        {
            var pieceInfo = _boardState.GetPiece(pos);
            _selectionInfo.Select(pieceInfo, pos);
        }
    }
}
