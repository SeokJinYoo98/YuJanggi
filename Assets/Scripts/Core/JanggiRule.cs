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


}