namespace Yujanggi.Core.Rule
{
    using Domain;
    using System;
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
            SelectionState selectPiece)
        {
            if (!selectPiece.HasSelection)
                throw new Exception("셀렉션이 없는데 길을 왜 찾지?");

            // 리스트 최소화!!!
            var candidates = _movementRule.CandidateWays(board, selectPiece);
            _palaceRule.ApplyPalaceRule(board, in selectPiece, candidates);

            board.AddMovable(candidates);
        }
    }
}