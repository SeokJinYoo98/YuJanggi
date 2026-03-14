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
            _palaceRule.ApplyPalaceRule(board, info, candidates);

            board.AddMovable(candidates);
        }
    }
    

}