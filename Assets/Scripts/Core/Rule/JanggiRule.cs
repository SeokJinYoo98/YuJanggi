namespace Yujanggi.Core.Rule
{
    using Domain;
    using System;
    using Yujanggi.Core.Board;
    using System.Collections.Generic;

    public class JanggiRule
    {
        private readonly MovementRule _movementRule;
        private readonly PalaceRule   _palaceRule;


        private readonly List<Pos>  _legalMoveBuffer = new(35);
        private SelectionState      _simulation;
        private SelectionState      _checkMate;
        public JanggiRule(PlayerTeam bottomPlayer)
        {
            _checkMate    = new (bottomPlayer);
            _simulation   = new(bottomPlayer);
            _movementRule = new();
            _palaceRule   = new();
        }

        public void FindWays(
            IBoardModel board,
            SelectionState selectionState)
        {
            if (!selectionState.HasSelection)
                throw new Exception("셀렉션이 없는데 길을 왜 찾지?");
            var candidates = FindCandidates(board, selectionState);
            FilterLegalMoves(board, selectionState, candidates);

            selectionState.SetMovable(candidates);
        }
        public bool IsKingInCheck(IBoardModel board, PlayerTeam team)
        {
            var kingPos = board.GetKingPos(team);

            for (int x = 0; x < board.WIDTH; ++x)
            {
                for (int z = 0; z < board.HEIGHT; ++z)
                {
                    var pos = new Pos(x, z);

                    if (!board.HasPiece(pos))
                        continue;

                    var piece = board.GetPiece(pos);

                    // 내 말이면 검사할 필요 없음
                    if (piece.Team == team)
                        continue;

                    _simulation.Clear();
                    _simulation.Select(piece, pos);

                    var ways = FindCandidates(board, _simulation);

                    foreach (var way in ways)
                    {
                        if (way.X == kingPos.X && way.Z == kingPos.Z)
                            return true;
                    }
                }
            }

            return false;
        }
        public bool HasAnyLegalMove(IBoardModel board, PlayerTeam defence)
        {
            for (int x = 0; x < board.WIDTH; ++x)
            {
                for (int z = 0; z < board.HEIGHT; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!board.HasPiece(pos)) continue;

                    var piece = board.GetPiece(pos);
                    if (piece.Team != defence) continue;

                    _simulation.Clear();
                    _simulation.Select(piece, pos);

                    var ways = FindCandidates(board, _simulation);
                    FilterLegalMoves(board, _simulation, ways);
                    
                    if (0 < ways.Count)
                    {
                        UnityEngine.Debug.Log($"Ways:{ways.Count}");
                        return false;
                    }
                        
                }
            }
            return true;
        }


        private List<Pos> FindCandidates(IBoardModel board, SelectionState selectionState)
        {
            var candidates = _movementRule.CandidateWays(board, selectionState);
            _palaceRule.ApplyPalaceRule(board, selectionState, candidates);
            return candidates;
        }
        private void FilterLegalMoves(IBoardModel board, SelectionState selection, List<Pos> candidates)
        {
            var fromPos = selection.SelectedPos;
            var myTeam = selection.SelectedPiece.Team;

            _legalMoveBuffer.Clear();

            foreach (var toPos in candidates)
            {
                var moveRecord = board.DoMove(fromPos, toPos);

                if (!IsKingInCheck(board, myTeam))
                    _legalMoveBuffer.Add(toPos);

                board.UndoMove(moveRecord);
            }

            candidates.Clear();
            candidates.AddRange(_legalMoveBuffer);
        }
    }
}