using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

namespace Yujanggi.Core.Movement
{
    public class PatternMovement : Movement
    {
        protected Step[][] _steps;
        public override List<Pos> FindWays(
            IBoardModel board,
            SelectionState selectPiece)
        {
            List<Pos> ways = new();

            foreach (var steps in _steps)
                ProcessDirection(ways, board, selectPiece.SelectedPiece.Team, selectPiece.BottomPlayer, selectPiece.SelectedPos, steps);

            return ways;
        }
        private void ProcessDirection(
            List<Pos> ways,
            IBoardModel board,
            PlayerTeam team,
            PlayerTeam bottom,
            Pos pos,
            Step[] steps)
        {
            int len = steps.Length;
            var dPos = pos;
            for (int j = 0; j < len; ++j)
            {
                dPos = ApplyStep(steps[j], team, bottom, dPos);

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
        private bool IsBlocked(IBoardModel board, PlayerTeam team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return result != StepResult.Empty;
        }
        private bool CanLand(IBoardModel board, PlayerTeam team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return (result == StepResult.Empty) || (result == StepResult.Enemy);
        }
    }
}
