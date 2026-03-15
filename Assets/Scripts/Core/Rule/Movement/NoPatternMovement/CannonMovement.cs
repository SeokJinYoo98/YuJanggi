using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Movement
{
    public class CannonMovement : Movement
    {
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };

        public override List<Pos> FindWays(
            IBoardState board,
            BoardInfo info)
        {
            List<Pos> ways = new();
            var bottom = board.BottomPlayer;
            var pos = info.Pos;
            var team = info.Piece.Team;

            foreach (var step in _steps)
            {
                bool bridge = false;

                while (true)
                {
                    var dPos = ApplyStep(step, team, bottom, pos);
                    if (!board.BoundaryCheck(dPos)) 
                        break;

                    if (!board.IsTherePiece(dPos, out var piece))
                    {
                        if (bridge)
                            ways.Add(dPos);
                        continue;
                    }

                    // 말 만났을 때
                    if (!bridge)
                    {
                        if (piece.Type != PieceType.Cannon)
                            bridge = true;

                        continue;
                    }

                    // 두번째 말
                    if (piece.Team != team && piece.Type != PieceType.Cannon)
                        ways.Add(dPos);
                    break;
                }
            }
            return ways;
        }
    }
}