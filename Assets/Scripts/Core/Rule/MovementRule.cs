using System.Collections.Generic;

namespace Yujanggi.Core.Rule
{
    using Board;
    using Movement;
    using Domain;

    public class MovementRule
    {
        Dictionary<PieceType, Movement> _rules;

        public MovementRule()
        {
            var palace = new PalaceMovement();
            _rules = new()
            {
                {PieceType.Soldier,     new SoldierMovement() },
                {PieceType.Chariot,     new ChariotMovement() },
                {PieceType.Cannon,      new CannonMovement() },
                {PieceType.Horse,       new Horsemovement() },
                {PieceType.Elephant,    new ElephantMovement() },
                {PieceType.King,        palace },
                {PieceType.Guard,       palace }
            };
        }

        public List<(int x, int z)> CandidateWays(
            IBoardState board,
            TurnInfo info)
        {
            if (!_rules.TryGetValue(info.Piece.Type, out var rule))
                return new List<(int, int)>();

            return rule.FindWays(board, info.Player, info.x, info.z);
        }
    }
}