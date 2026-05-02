using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;
using Yujanggi.Runtime.GameSession;

namespace Yujanggi.Runtime.Controller
{
    public class AIController : IPlayerController, IAIController
    {
        public PlayerTeam Team { get; }
        public event Action<Pos, Pos> OnMoveRequest;
        public bool IsLocal() => false;
       
        private readonly IJanggiRule        _rule;
        private readonly IBoardModel        _boardModel;
        private readonly Selection          _selection;
        private readonly System.Random      _rand = new();

        private readonly List<MoveCandidate> _candidates = new(17);
        private int _selectedCandidateIndex = -1;

        public AIController(IJanggiRule rule, IBoardModel board, PlayerTeam team, ICoroutineRunner runner)
        {
            Team                = team;
            _runner             = runner;
            _rule               = rule;
            _boardModel         = board;
            _selection          = new Selection();
        }
        public void BindEvents() { }
        public void UnBindEvents() { }
        public bool TryThink()
        {
            _candidates.Clear();
            _selectedCandidateIndex = -1;

            int pieceCount = 0;
            int myPieceCount = 0;
            int legalCount = 0;

            for (int x = 0; x < _boardModel.WIDTH; ++x)
            {
                for (int z = 0; z < _boardModel.HEIGHT; ++z)
                {
                    var from = new Pos(x, z);

                    if (!_boardModel.HasPiece(from))
                        continue;

                    pieceCount++;

                    var piece = _boardModel.GetPiece(from);

                    if (piece.Team != Team)
                        continue;

                    myPieceCount++;

                    _selection.Clear();
                    _selection.FromPos = from;

                    _rule.FindWays(_boardModel, _selection);

                    var movable = _selection.LegalCells;

                    if (movable != null)
                        legalCount += movable.Count;

  
                    if (movable == null || movable.Count == 0)
                        continue;

                    var ways = new List<Pos>(movable.Count);
                    ways.AddRange(movable);

                    _candidates.Add(new MoveCandidate(piece, from, ways));
                }
            }

            if (_candidates.Count == 0)
                return false;

            _selectedCandidateIndex = _rand.Next(0, _candidates.Count);
            return true;
        }
        public bool TryGetSelectedMove()
        {
            if (_selectedCandidateIndex < 0 || _selectedCandidateIndex >= _candidates.Count)
                return false;

            Pos from = SelectPiece();
            Pos to   = SelectCell();
            OnMoveRequest?.Invoke(from, to);
            return true;
        }
        private Pos SelectPiece()
        {  
            var selected = _candidates[_selectedCandidateIndex];
            return selected.From;
        }
        private Pos SelectCell() 
        {
            var selected = _candidates[_selectedCandidateIndex];
            int random = _rand.Next(0, selected.Ways.Count);
            return selected.Ways[random];
        }

        private readonly ICoroutineRunner _runner;
        private Coroutine                 _aiRoutine;
        private IEnumerator ProcessAITurn()
        {
            if (!TryThink()) yield break;

            yield return new WaitForSeconds(1f);

            if (!TryGetSelectedMove()) yield break;
        }

        public void BeginTurn()
        {
            Debug.Log("AI 비긴턴");
            if (_aiRoutine != null)
            {
                _runner.Stop(_aiRoutine);
                _aiRoutine = null;
            }
            _aiRoutine = _runner.Run(ProcessAITurn());
            Debug.Log("AI 비긴끝");
        }

        public void EndTurn()
        {
            Debug.Log("AI엔드턴");
            if (_aiRoutine == null) return;

            _runner.Stop(_aiRoutine);
            _aiRoutine = null;
        }
        private readonly struct MoveCandidate
        {
            public PieceModel   Piece { get; }
            public Pos          From { get; }
            public List<Pos>    Ways { get; }

            public MoveCandidate(PieceModel piece, Pos from, List<Pos> ways)
            {
                Piece = piece;
                From  = from;
                Ways  = ways;
            }
        }
    }
}
