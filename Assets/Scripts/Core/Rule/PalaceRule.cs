
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

        public void ApplyPalaceRule(IBoardState board, in BoardInfo info, List<Pos> candidates)
        {
            var type = info.Piece.Type;
            if (_addRule.Contains(type) && board.IsPalace(info.Pos))
            {
                var ways = _palaceMovement.FindWays(board, info);
                candidates.AddRange(ways);
            }

            if (_filterRule.Contains(type))
                Filter(board, info, candidates, type);
        }



        private void Filter(IBoardState board, in BoardInfo info, List<Pos> ways, PieceType type)
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
        private void FilterSoldier(IBoardState board, in BoardInfo info, List<Pos> ways)
        {
  
            int z = info.Pos.Z;
            var isBottom = BoardHelper.IsBottomPlayer(board, info.Piece.Team);
            if (isBottom)
                ways.RemoveAll(pos => pos.Z < z);
            else
                ways.RemoveAll(pos => pos.Z > z);
        }
        private void FilterKingGuard(IBoardState board, List<Pos> ways)
        {
            ways.RemoveAll(pos => !board.IsPalace(pos));
        }
    }
}