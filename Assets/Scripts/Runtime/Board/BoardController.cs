using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;

    public class BoardController : MonoBehaviour
    {
        private BoardView   _boardView;
        private BoardModel  _boardModel;
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
            _boardModel      = new(bottomPlayer);
            _boardView.SpawnPieceView(_boardModel);
        }
        public  BoardActionResult  HandleCellClick(Pos pos, PlayerTeam turn)
        {
            if (!_boardModel.IsInside(pos))
                return BoardActionResult.None;

            if (!_selectionInfo.HasSelection)
                return HandleSelectPiece(pos, turn);

            return HandleSelectedClick(pos, turn);
        }
        private BoardActionResult  HandleSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!CanSelectPiece(pos, turn))
                return BoardActionResult.SelectFailed;

            SelectPeice(pos);
            FindWays(pos);

            return BoardActionResult.SelectSuccess;
        }
        private BoardActionResult  HandleSelectedClick(Pos pos, PlayerTeam turn)
        {
            if (CanSelectPiece(pos, turn))
                ReselectPiece(pos, turn);

            if (!_boardModel.IsMovable(pos))
            {
                ClearSelection();
                return BoardActionResult.MoveFailed;
            }


            return MoveSelectedPiece(pos, turn);
        }
        private BoardActionResult  ReselectPiece(Pos pos, PlayerTeam turn)
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
        private BoardActionResult  MoveSelectedPiece(Pos pos, PlayerTeam turn)
        {
            var targetPiece = _boardModel.GetPiece(pos);
            var result = targetPiece.IsNone
                ? BoardActionResult.MoveSuccess
                : BoardActionResult.CaptureSuccess;

            var fromPos = _selectionInfo.SelectedPos;
            _boardModel.SetPiece(pos, _boardModel.GetPiece(fromPos));
            _boardModel.SetPiece(fromPos, PieceInfo.None);

            _boardView.MovePieceView(fromPos, pos, out var killedView);

            ClearSelection();

            return result;
        }

        private void FindWays(Pos pos)
        {
            _boardModel.ClearMovable();
            _janggiRule.FindWays(_boardModel, _selectionInfo);
            _boardView.ShowHighlights(pos, _boardModel.MovableCells);
        }
        private void ClearSelection()
        {
            _selectionInfo.Clear();
            _boardView.HideHighlights();
        }

        private bool CanSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!_boardModel.HasPiece(pos))
                return false;

            var pieceInfo = _boardModel.GetPiece(pos);
            return pieceInfo.Team == turn;
        }
        private void SelectPeice(Pos pos)
        {
            var pieceInfo = _boardModel.GetPiece(pos);
            _selectionInfo.Select(pieceInfo, pos);
        }
    }
}
