using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Movement
{
    public class CannonMovement : Movement
    {
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };

        public override List<(int x, int z)> FindWays(
            IBoardState board, 
            PlayerType team, 
            int x, int z)
        {
            List<(int x, int z)> ways = new();

            var bottom = board.BottomPalyer;
            foreach (var step in _steps)
            {
                bool bridge = false;

                int dx = x;
                int dz = z;

                while (true)
                {
                    ApplyStep(step, team, bottom, ref dx, ref dz);

                    if (!board.BoundaryCheck(dx, dz)) 
                        break;

                    if (!board.IsTherePiece(dx, dz, out var otherTeam, out var type))
                    {
                        if (bridge)
                            ways.Add((dx, dz));
                        continue;
                    }

                    // 말 만났을 때
                    if (!bridge)
                    {
                        if (type != PieceType.Cannon)
                            bridge = true;

                        continue;
                    }

                    // 두번째 말
                    if (otherTeam != team && type != PieceType.Cannon)
                        ways.Add((dx, dz));
                    break;
                }
            }
            return ways;
        }
    }
}