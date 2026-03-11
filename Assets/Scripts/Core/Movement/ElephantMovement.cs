
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{

    public class ElephantMovement : Movement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();
            for (int i = 0; i < Dirs.Length; ++i)
            {
                var dir = Dirs[i];
                int dx = x + dir.x;
                int dz = z + dir.z;

                var result = CheckCell(board, team, (dx, dz));
                if (result != StepResult.Empty) continue;

                DiagonalMove(board, i, team, dx, dz, ways);
            }

            return ways;
        }

        private void DiagonalMove(IBoardState board, int i, PlayerType team, int x, int z, in List<(int x, int z)> ways)
        {
            if (3 < i) return;
            // 우 -> 우상4 우하5
            // 좌 -> 좌상6 좌하7
            // 상 -> 좌상6 우상4
            // 하 -> 좌하7 우하5

            (int x, int z) diag1 = Dirs[7];
            (int x, int z) diag2 = Dirs[5];
            if (i == 0)
            {
                diag1 = Dirs[4];
                diag2 = Dirs[5];
            }
            else if (i == 1)
            {
                diag1 = Dirs[6];
                diag2 = Dirs[7];
            }
            else if (i == 2)
            {
                diag1 = Dirs[6];
                diag2 = Dirs[4];
            }
            int dx = x + diag1.x;
            int dz = z + diag1.z;
            var result = CheckCell(board, team, dx, dz);
            if (result == StepResult.Empty)
            {
                dx += diag1.x;
                dz += diag1.z;
                var r2 = CheckCell(board, team, dx, dz);
                if (r2 == StepResult.Empty || r2 == StepResult.Enemy)
                    ways.Add((dx, dz));
            }

            dx = x + diag2.x;
            dz = z + diag2.z;
            result = CheckCell(board, team, dx, dz);
            if (result == StepResult.Empty)
            {
                dx += diag2.x;
                dz += diag2.z;
                var r2 = CheckCell(board, team, dx, dz);
                if (r2 == StepResult.Empty || r2 == StepResult.Enemy)
                    ways.Add((dx, dz));
            }
            // 0 1 -> LR
            // 2 3 -> FB

        }
    }
}