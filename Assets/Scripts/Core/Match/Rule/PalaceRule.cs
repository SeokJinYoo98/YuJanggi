
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Rule
{
    using Match.Movement;
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

        public void ApplyPalaceRule(IBoardModel board, in SelectionState selectInfo, List<Pos> candidates)
        {
            var selectedPiece = selectInfo.SelectedPiece;
            var type = selectedPiece.Type;
            var pos = selectInfo.SelectedPos;
            if (_addRule.Contains(type) && board.IsPalace(pos))
            {
                var ways = _palaceMovement.FindWays(board, selectInfo);
                candidates.AddRange(ways);
            }

            if (_filterRule.Contains(type))
                Filter(board, selectInfo, candidates, type);
        }



        private void Filter(IBoardModel board, in SelectionState selectPiece, List<Pos> ways, PieceType type)
        {
            switch(type)
            {
                case PieceType.Soldier:
                    FilterSoldier(board, selectPiece, ways);
                    break;

                case PieceType.King:
                case PieceType.Guard:
                    FilterKingGuard(board, ways);
                    break;
            }
        }
        private void FilterSoldier(IBoardModel board, SelectionState selectInfo, List<Pos> ways)
        {

            int z = selectInfo.SelectedPos.Z;

            if (selectInfo.Team == PlayerTeam.Cho)
                ways.RemoveAll(pos => pos.Z < z);
            else
                ways.RemoveAll(pos => pos.Z > z);
        }
        private void FilterKingGuard(IBoardModel board, List<Pos> ways)
        {
            ways.RemoveAll(pos => !board.IsPalace(pos));
        }
    }
}