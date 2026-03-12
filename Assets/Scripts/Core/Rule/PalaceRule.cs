
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Rule
{
    public class PalaceRule
    {
        // 추가 대상
        private readonly HashSet<PieceType> _addRule = new() 
                { PieceType.Cannon, PieceType.Chariot, PieceType.Soldier, PieceType.King, PieceType.Guard };
        // 필터 대상
        private readonly HashSet<PieceType> _filterRule = new() 
                { PieceType.King, PieceType.Guard };


        public PalaceRule()
        {

        }

        public void ApplyPalaceRule(IBoardState board, TurnInfo info, List<(int x, int z)> candidates)
        {
            var type = info.Piece.Type;
            if (_addRule.Contains(type))
                AddCandidateMoves(board, info, candidates);

            if (_filterRule.Contains(type))
                FilterCandidateMoves(board, info, candidates);

        }

        private void AddCandidateMoves(IBoardState board, TurnInfo info, List<(int x, int z)> candidates)
        {

        }
        private void FilterCandidateMoves(IBoardState board, TurnInfo info, List<(int x, int z)> candidates)
        {
            // 순회 수정 오류
            //foreach(var way in candidates)
            //{
            //    if (!board.IsPalace(way.x, way.z))
            //        candidates.Remove(way);
            //}

            candidates.RemoveAll(pos => !board.IsPalace(pos.x, pos.z));
        }
    }
}