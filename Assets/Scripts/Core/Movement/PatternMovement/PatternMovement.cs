using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class PatternMovement : Movement
    {
        protected Step[][] _steps;
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();

            foreach (var steps in _steps)
                ProcessDirection(ways, board, team, x, z, steps);

            return ways;
        }
        private void ProcessDirection(
            List<(int x, int z)> ways,
            IBoardState board,
            PlayerType team,
            int x,
            int z,
            Step[] steps)
        {
            int dx = x;
            int dz = z;

            int len = steps.Length;

            for (int j = 0; j < len; ++j)
            {
                ApplyStep(steps[j], team, ref dx, ref dz);

                if (j < len - 1)
                {
                    if (IsBlocked(board, team, dx, dz))
                        return;
                }
                else
                {
                    if (CanLand(board, team, dx, dz))
                        ways.Add((dx, dz));
                }
            }
        }
        private bool IsBlocked(IBoardState board, PlayerType team, int dx, int dz)
        {
            var result = CheckCell(board, team, dx, dz);
            return result != StepResult.Empty;
        }
        private bool CanLand(IBoardState board, PlayerType team, int dx, int dz)
        {
            var result = CheckCell(board, team, dx, dz);
            return (result == StepResult.Empty) || (result == StepResult.Enemy);
        }
    }
}
