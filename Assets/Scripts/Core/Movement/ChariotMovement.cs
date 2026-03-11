using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class ChariotMovement : Movement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();
            for (int i = 0; i < Dirs.Length; ++i)
            {
                var dir = Dirs[i];

                int dx = x;
                int dz = z;
                while (true)
                {
                    dx += dir.x;
                    dz += dir.z;

                    var result = CheckCell(board, team, dx, dz);

                    if (result == StepResult.Block || result == StepResult.Team)
                        break;

                    ways.Add((dx, dz));
                    if (result == StepResult.Enemy)
                        break;
                }
            }
            return ways;
        }
    }
}