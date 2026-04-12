
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Core.Rule;

namespace Yujanggi.Runtime.Controller
{
    public class AIPolicy
    {

    }
    public class AIController : IPlayerController, IAIController
    {
        public PlayerTeam Team { get; }
        public event Action<Pos, Pos> OnMoveRequest;


        private readonly IJanggiRule        _rule;
        private readonly IBoardModel        _boardModel;
        private Selection _sel;
        private readonly System.Random _rand = new();

        private readonly List<MoveCandidate> _candidates = new(17);
        private int _selectedCandidateIndex = -1;

        public AIController(IJanggiRule rule, IBoardModel board, PlayerTeam team, ICoroutineRunner runner)
        {
            Team        = team;
            _runner     = runner;
           
            _rule       = rule;
            _boardModel = board;
            _sel        = new Selection();
        }
        
        public bool TryThink()
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
                    if (piece.Team != Team)
                        continue;

                    _sel.Clear();
                    _sel.FromPos = from;

                    _rule.FindWays(_boardModel, _sel);

   
                    var movable = _sel.LegalCells;
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
        public void BindEvents(IGameInputHandler manager)
        {
            //OnMoveRequest += manager.HandleMoveRequest;
        }
        public void UnBindEvents(IGameInputHandler manager)
        {
            //OnMoveRequest -= manager.HandleMoveRequest;
        }

        private readonly ICoroutineRunner _runner;
        private Coroutine _aiRoutine;
        private IEnumerator ProcessAITurn()
        {
            if (!this.TryThink())
                yield break;

            yield return new WaitForSeconds(1f);

            if (!this.TryGetSelectedMove())
                yield break;
        }

        public void BeginTurn()
        {
            if (_aiRoutine != null) return;
            _aiRoutine = _runner.Run(ProcessAITurn());
        }

        public void EndTurn()
        {
            if (_aiRoutine == null) return;

            _runner.Stop(_aiRoutine);
            _aiRoutine = null;
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
