using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;

    public class BoardController : MonoBehaviour
    {
        public event Action<MoveContext> OnMove;

        private BoardView   _boardView;
        private BoardModel  _boardModel;

        private JanggiRule          _janggiRule;
        private SelectionState      _selection;
       
        private void Awake()
        {
            _boardView = GetComponent<BoardView>();
            _janggiRule = new();
        }
        public void StartGame(PlayerTeam bottom)
        {
            _boardModel  = new(bottom); BoardInitializer.SetUpPieces(_boardModel, bottom);
            _selection   = new(bottom);
            _boardView.SpawnPieceView(_boardModel);
        }
        public  BoardActionResult  HandleCellClick(Pos pos, PlayerTeam turn)
        {
            if (!_boardModel.IsInside(pos))
                return BoardActionResult.None;

            if (!_selection.HasSelection)
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
                return ReselectPiece(pos, turn);
      
            if (!_selection.IsMovable(pos))
            {
                ClearSelection();
                return BoardActionResult.MoveFailed;
            }

            return MoveSelectedPiece(pos, turn);
        }
        private BoardActionResult ReselectPiece(Pos pos, PlayerTeam turn)
        {
            if (pos == _selection.SelectedPos)
                return BoardActionResult.None;
            SelectPeice(pos);
            FindWays(pos);
            return BoardActionResult.Reselect;
        }
        private BoardActionResult  MoveSelectedPiece(Pos toPos, PlayerTeam turn)
        {
            var fromPos = _selection.SelectedPos;
            var record = _boardModel.DoMove(fromPos, toPos);
            _boardView.DoMove(fromPos, toPos, out var capturedView);
            
            RaiseMove(new (record, capturedView));
            ClearSelection();

            return record.IsCapture 
                ? BoardActionResult.CaptureSuccess
                : BoardActionResult.MoveSuccess;
        }
        private void FindWays(Pos pos)
        {
            _janggiRule.FindWays(_boardModel, _selection);
            _boardView.ShowHighlights(pos, _selection.MovableCells);
        }
        private void ClearSelection()
        {
            _selection.Clear();
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
            _selection.Select(pieceInfo, pos);
        }


        // 이벤트 관련
        private void RaiseMove(MoveContext context)
        {
            OnMove?.Invoke(context);
        }
    }
}
