namespace Yujanggi.Core.Rule
{
    using Domain;
    using System.Collections.Generic;
    using Yujanggi.Core.Board;
    using Yujanggi.Core.Movement;

    public class JanggiRule
    {
        private MovementRule _movementRule;
        public JanggiRule()
        {
            _movementRule = new();
        }

        public void FindWays(
            IBoardState board,
            TurnInfo info)
        {
            board.ClearMovable();

            var candidate = _movementRule.CandidateWays(board, info);
            
            foreach(var way in candidate)
            {
                board.AddMovable(way.x, way.z);
            }
        }
    }
    public class MovementRule
    {
        Dictionary<PieceType, Movement> _rules;

        public MovementRule()
        {
            _rules = new()
            {
                {PieceType.Soldier, new SoldierMovement() },
                {PieceType.Chariot, new ChariotMovement() },
                {PieceType.Cannon, new CannonMovement() }
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