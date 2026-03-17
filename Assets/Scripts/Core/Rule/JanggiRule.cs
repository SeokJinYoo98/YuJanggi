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

        private SelectionState _simulation;
        public JanggiRule(PlayerTeam bottomPlayer)
        {
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
            JanggunRule(board, selectionState, candidates);

            selectionState.SetMovable(candidates);
        }
       
        private List<Pos> FindCandidates(IBoardModel board, SelectionState selectionState)
        {
            var candidates = _movementRule.CandidateWays(board, selectionState);
            _palaceRule.ApplyPalaceRule(board, selectionState, candidates);
            return candidates;
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
        private void JanggunRule(IBoardModel board, SelectionState selection, List<Pos> candidates)
        {
            var fromPos = selection.SelectedPos;
            var myTeam = selection.SelectedPiece.Team;

            var legalMoves = new List<Pos>();

            foreach (var toPos in candidates)
            {
                var moveRecord = board.DoMove(fromPos, toPos);

                if (!IsKingInCheck(board, myTeam))
                    legalMoves.Add(toPos);

                board.UndoMove(moveRecord);
            }

            candidates.Clear();
            candidates.AddRange(legalMoves);
        }
    }
}