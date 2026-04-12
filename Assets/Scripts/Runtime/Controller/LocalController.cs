using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;
using UnityEngine;
namespace Yujanggi.Runtime.Controller
{
    public class LocalController : IPlayerController, ILocalPlayer
    {
        public  PlayerTeam              Team { get; }
        private bool _isTurn;
        private readonly IInputHandler  _input;
        private readonly IBoardModel    _board;
        private readonly IJanggiRule    _rule;

        private Selection               _selection;

        private event Action<int?, IReadOnlyList<Pos>, IReadOnlyList<Pos>> OnSelectionChanged;
        public event Action<Pos, Pos> OnMoveRequest;
        public LocalController(IJanggiRule rule, IBoardModel board, PlayerTeam team, IInputHandler input)
        {
            Team        = team;
            _board      = board;
            _rule       = rule;
            _input      = input;

            _selection = new Selection();

            _isTurn = false;
        }
        public void BindEvents(IGameInputHandler manager)
        {
            _input.OnBoardClicked += HandleClick;
            OnSelectionChanged    += manager.HandleSelectionChanged;
        }

        public void UnBindEvents(IGameInputHandler manager)
        {
            _input.OnBoardClicked -= HandleClick;
            OnSelectionChanged    -= manager.HandleSelectionChanged;
        }

        public void BeginTurn()
        {
            _isTurn = true;
        }

        public void EndTurn()
        {
            _isTurn = false;
        }
        private void HandleClick(Pos pos)
        {
            if (!CanHandleClick(pos))
                return;

            if (!_selection.HasSelection)
            {
                SelectPiece(pos);
                return;
            }

            if (TryMovePiece(pos))
                return;

            if (TryReselectPiece(pos))
                return;

            ClearSelection();
        }

        private bool CanHandleClick(Pos pos)
            => _isTurn && _board.IsInside(pos);

        private void SelectPiece(Pos pos)
        {
            if (!TryGetOwnPiece(pos, out var piece))
                return;

            _selection.Clear();
            _selection.FromPos = pos;
            _rule.FindWays(_board, _selection);

            OnSelectionChanged?.Invoke(piece.Id, _selection.LegalCells, _selection.IllegalCells);
        }

        private bool TryMovePiece(Pos toPos)
        {
            if (_selection.FromPos == toPos)
                return false;

            if (!_selection.IsMovable(toPos))
                return false;

            OnMoveRequest?.Invoke(_selection.FromPos, toPos);

            return true;
        }

        private bool TryReselectPiece(Pos pos)
        {
            if (pos == _selection.FromPos) return false;
            if (!_board.HasPiece(pos)) return false;

            SelectPiece(pos);
            return true;
        }

        private bool TryGetOwnPiece(Pos pos, out PieceModel piece)
        {
            piece = default;
            if (!_board.HasPiece(pos))
                return false;

            piece = _board.GetPiece(pos);
            return piece.Team == Team;
        }

        private void ClearSelection()
        {
            _selection.Clear();
            OnSelectionChanged?.Invoke(null, _selection.LegalCells, _selection.IllegalCells);
        }
    }
}