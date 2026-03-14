using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class ChariotMovement : Movement
    {
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();
            var bottom = board.BottomPalyer;
            foreach (var step in _steps)
            {
                int dx = x; int dz = z;
                while (true)
                {
                    ApplyStep(step, team, bottom, ref dx, ref dz);
                    if (!board.BoundaryCheck(dx, dz)) break;
                    if (board.IsTherePiece(dx, dz, out var team2, out var _))
                    {
                        if (team2 != team)
                            ways.Add((dx, dz));
                        break;
                    }
                    ways.Add((dx, dz));
                }
            }
            return ways;
        }


    }
}