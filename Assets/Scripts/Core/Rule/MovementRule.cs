using System.Collections.Generic;

namespace Yujanggi.Core.Rule
{
    using Board;
    using Movement;
    using Domain;

    public class MovementRule
    {
        readonly Dictionary<PieceType, Movement> _rules;

        public MovementRule()
        {
            var kg = new KingGuardMovement();
            _rules = new()
            {
                {PieceType.Soldier,     new SoldierMovement() },
                {PieceType.Chariot,     new ChariotMovement() },
                {PieceType.Cannon,      new CannonMovement() },
                {PieceType.Horse,       new Horsemovement() },
                {PieceType.Elephant,    new ElephantMovement() },
                {PieceType.King,        kg },
                {PieceType.Guard,       kg }
            };
        }

        public List<(int x, int z)> CandidateWays(
            IBoardState board,
            in BoardInfo info)
        {
            if (!_rules.TryGetValue(info.Piece.Type, out var rule))
                return new List<(int, int)>();

            return rule.FindWays(board, info.Team, info.x, info.z);
        }
    }
}