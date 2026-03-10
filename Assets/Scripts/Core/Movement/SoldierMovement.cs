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
                int dx = x + Dirs[i].x;
                int dz = z + Dirs[i].z * forward;
            }

            return ways;
        }
    }
}