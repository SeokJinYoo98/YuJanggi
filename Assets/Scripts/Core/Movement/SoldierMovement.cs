using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class SoldierMovement : Movement
    {
        public override List<(int x, int z)> FindWays(
            IBoardState board,
            PlayerType team,
            int x, int z)
        {
            List<(int, int)> ways = new();

            int forward = Forward(team);
            for (int i = 0; i < 3; ++i)
            {
                var dir = Dirs[i];
                int dx = x + dir.x;
                int dz = z + dir.z * forward;

                var result = CheckCell(board, team, dx, dz);
                if (result != StepResult.Block && result != StepResult.Team)
                    ways.Add((dx, dz));

            }
            return ways;
        }
    }
}