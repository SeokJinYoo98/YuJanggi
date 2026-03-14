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
            in BoardInfo info)
        {
            board.ClearMovable();

            // 리스트 최소화!!!
            var candidates = _movementRule.CandidateWays(board, info);
            _palaceRule.ApplyPalaceRule(board, in info, candidates);

            board.AddMovable(candidates);
        }
    }
}