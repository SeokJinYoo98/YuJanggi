using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class PatternMovement : Movement
    {
        protected Step[][] _steps;
        public override List<Pos> FindWays(
            IBoardState board,
            BoardInfo info)
        {
            List<Pos> ways = new();

            foreach (var steps in _steps)
                ProcessDirection(ways, board, info.Piece.Team, info.Pos, steps);

            return ways;
        }
        private void ProcessDirection(
            List<Pos> ways,
            IBoardState board,
            PlayerType team,
            Pos pos,
            Step[] steps)
        {
            int len = steps.Length;
            var bottom = board.BottomPlayer;
            for (int j = 0; j < len; ++j)
            {
                var dPos = ApplyStep(steps[j], team, bottom, pos);

                if (j < len - 1)
                {
                    if (IsBlocked(board, team, dPos))
                        return;
                }
                else
                {
                    if (CanLand(board, team, dPos))
                        ways.Add((dPos));
                }
            }
        }
        private bool IsBlocked(IBoardState board, PlayerType team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return result != StepResult.Empty;
        }
        private bool CanLand(IBoardState board, PlayerType team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return (result == StepResult.Empty) || (result == StepResult.Enemy);
        }
    }
}
