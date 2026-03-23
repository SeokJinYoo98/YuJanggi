using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;
    using Yujanggi.Runtime.Piece;

    public class BoardController : MonoBehaviour
    {
        [SerializeField] private PieceManager _pieces;

        public event Action<MoveContext> OnMoved;

        private BoardView   _boardView;
        private BoardModel  _boardModel;

        private JanggiRule          _janggiRule;
        private SelectionState      _selection;

        private void Awake()
        {
            _boardView = GetComponent<BoardView>();
            
        }
        public void StartGame(PlayerTeam bottom)
        {
            _boardModel  = new(bottom); BoardInitializer.SetUpPieces(_boardModel, bottom);
            _selection   = new(bottom);
            _janggiRule  = new(bottom);
            _pieces.SpawnPieces(_boardModel, bottom);
        }
        public void ResetGame(PlayerTeam bottom)
        {
            _boardModel.ResetBoard();
            BoardInitializer.SetUpPieces(_boardModel, bottom);
            _pieces.ResetViews(_boardModel);
            ClearSelection();
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
        private BoardActionResult  ReselectPiece(Pos pos, PlayerTeam turn)
        {
            if (pos == _selection.SelectedPos)
            {
                ClearSelection(); 
                return BoardActionResult.SelectFailed;
            }

            SelectPeice(pos);
            FindWays(pos);
            return BoardActionResult.Reselect;
        }
        private BoardActionResult  MoveSelectedPiece(Pos toPos, PlayerTeam turn)
        {
            var fromPos = _selection.SelectedPos;
            var record = _boardModel.DoMove(fromPos, toPos);
            _pieces.DoMove(in record);

            var otherTeam = turn == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
            var isJanggun = IsJanggun(otherTeam);
            var isEnd     = isJanggun && HasAnyLegalMove(otherTeam);
            RaiseMove(new (record, isJanggun, isEnd));
            ClearSelection();

            return record.IsCapture 
                ? BoardActionResult.CaptureSuccess
                : BoardActionResult.MoveSuccess;
        }
        private void FindWays(Pos pos)
        {
            _janggiRule.FindWays(_boardModel, _selection);
            _boardView.Highlight(_selection.MovableCells);
        }
        private void ClearSelection()
        {
            _selection.Clear();
            _boardView.UnHighlight();
            _pieces.UnHighlight();
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
            ClearSelection();
            var pieceInfo = _boardModel.GetPiece(pos);
            _selection.Select(pieceInfo, pos);
            _pieces.HighlightPiece(pieceInfo.Id);
        }

        // 이벤트 관련
        private void RaiseMove(in MoveContext context)
        {
            OnMoved?.Invoke(context);
        }

        // Undo 
        public void Undo(in MoveContext context)
        {
            ClearSelection();
            var record = context.Record;
            _boardModel.UndoMove(record);
            _pieces.UnDoMove(record);
        }

        private bool IsJanggun(PlayerTeam otherTeam)
            => _janggiRule.IsKingInCheck(_boardModel, otherTeam);
        private bool HasAnyLegalMove(PlayerTeam otherTeam)
            => _janggiRule.HasAnyLegalMove(_boardModel, otherTeam);

        public void HandiCap()
        {
            ClearSelection();
        }
    }
}
