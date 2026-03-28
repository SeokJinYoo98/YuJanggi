



using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;
using UnityEngine;
namespace Yujanggi.Core.AI
{

    using Participant;
    public class AIPolicy
    {

    }
    public class AIController : IParticipantController
    {
        private bool _canInput = false;
        public event Action<Pos> OnBoardClicked;

        private readonly IJanggiRule _rule;
        private readonly IBoardModel _boardModel;
        private readonly SelectionState _selection;
        private readonly System.Random _rand = new();

        public bool CanInput => _canInput;

        private readonly List<MoveCandidate> _candidates = new(17);
        private int _selectedCandidateIndex = -1;

        public void SetInputEnabled(bool enabled)
            => _canInput = enabled;

        public AIController(IJanggiRule rule, IBoardModel model, PlayerTeam bottom)
        {
            _rule = rule;
            _boardModel = model;
            _selection = new SelectionState(bottom);
        }
        
        public bool TryThink(PlayerTeam team)
        {
            _candidates.Clear();
            _selectedCandidateIndex = -1;

            int width = _boardModel.WIDTH;
            int height = _boardModel.HEIGHT;

            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < height; ++z)
                {
                    var from = new Pos(x, z);

                    if (!_boardModel.HasPiece(from))
                        continue;

                    var piece = _boardModel.GetPiece(from);
                    if (piece.Team != team)
                        continue;

                    _selection.Clear();
                    _selection.Select(piece, from);

                    _rule.FindWays(_boardModel, _selection);

                    var movable = _selection.MovableCells;
                    if (movable == null || movable.Count == 0)
                        continue;

                    var ways = new List<Pos>(movable.Count);
                    ways.AddRange(movable);

                    _candidates.Add(new MoveCandidate(piece, from, ways));
                }
            }

            Debug.Log("Think");
            return _candidates.Count > 0;
        }

        public bool TrySelectPiece()
        {
            if (!_canInput)
                return false;

            if (_candidates.Count == 0)
                return false;

            _selectedCandidateIndex = _rand.Next(0, _candidates.Count);

            var selected = _candidates[_selectedCandidateIndex];
            OnBoardClicked?.Invoke(selected.From);
            Debug.Log("Select Piece");
            return true;
        }

        public bool TrySelectCell()
        {
            if (!_canInput)
                return false;

            if (_selectedCandidateIndex < 0 || _selectedCandidateIndex >= _candidates.Count)
                return false;

            var selected = _candidates[_selectedCandidateIndex];
            if (selected.Ways == null || selected.Ways.Count == 0)
                return false;

            int random = _rand.Next(0, selected.Ways.Count);
            OnBoardClicked?.Invoke(selected.Ways[random]);

            Debug.Log("Select Cell");
            return true;
        }

        private readonly struct MoveCandidate
        {
            public PieceModel Piece { get; }
            public Pos From { get; }
            public List<Pos> Ways { get; }

            public MoveCandidate(PieceModel piece, Pos from, List<Pos> ways)
            {
                Piece = piece;
                From = from;
                Ways = ways;
            }
        }
    }
}
