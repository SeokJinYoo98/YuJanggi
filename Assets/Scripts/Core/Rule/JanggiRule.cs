namespace Yujanggi.Core.Rule
{
    using Domain;
    using Yujanggi.Core.Board;
    

    public class JanggiRule
    {
        private MovementRule _movementRule;
        private PalaceRule   _palaceRule;
        public JanggiRule()
        {
            
            _movementRule = new();
            _palaceRule = new();
        }

        public void FindWays(
            IBoardState board,
            TurnInfo info)
        {
            board.ClearMovable();

            var candidates = _movementRule.CandidateWays(board, info);
            
            // [TODO] 필터 궁 룰 추 가.
            _palaceRule.ApplyPalaceRule(board, info, candidates);


            foreach (var way in candidates)
            {
                board.AddMovable(way.x, way.z);
            }
        }
    }
    

}