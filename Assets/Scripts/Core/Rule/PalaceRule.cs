
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Rule
{
    using Movement;
    using Utills.Board;
    public class PalaceRule
    {
        private PalaceMovement _palaceMovement;
        private readonly HashSet<PieceType> _addRule = new()
                { PieceType.Cannon, PieceType.Chariot, PieceType.Soldier, PieceType.King, PieceType.Guard };
        // 필터 대상
        private readonly HashSet<PieceType> _filterRule = new()
                { PieceType.King, PieceType.Guard, PieceType.Soldier };

        public PalaceRule()
        {
            _palaceMovement = new();
        }

        public void ApplyPalaceRule(IBoardState board, in TurnInfo info, List<(int x, int z)> candidates)
        {
            var type = info.Piece.Type;
            if (_addRule.Contains(type) && board.IsPalace(info.x, info.z))
            {
                var ways = _palaceMovement.FindWays(board, info.Player, info.x, info.z);
                candidates.AddRange(ways);
            }

            if (_filterRule.Contains(type))
                Filter(board, info, candidates, type);
        }



        private void Filter(IBoardState board, in TurnInfo info, List<(int x, int z)> ways, PieceType type)
        {
            switch(type)
            {
                case PieceType.Soldier:
                    FilterSoldier(board, info, ways);
                    break;

                case PieceType.King:
                case PieceType.Guard:
                    FilterKingGuard(board, ways);
                    break;
            }
        }
        private void FilterSoldier(IBoardState board, in TurnInfo info, List<(int x, int z)> ways)
        {
  
            int z = info.z;
            var isBottom = BoardHelper.IsBottomPlayer(board, info.Player);
            if (isBottom)
                ways.RemoveAll(pos => pos.z < z);
            else
                ways.RemoveAll(pos => pos.z > z);
        }
        private void FilterKingGuard(IBoardState board, List<(int x, int z)> ways)
        {
            ways.RemoveAll(pos => !board.IsPalace(pos.x, pos.z));
        }
    }
}