using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class ChariotMovement : Movement
    {
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };
        public override List<Pos> FindWays(
            IBoardState board,
            BoardInfo boardInfo)
        {
            List<Pos> ways = new();
            var bottom = board.BottomPlayer;
            var team = boardInfo.Piece.Team;
            var pos = boardInfo.Pos;
            foreach (var step in _steps)
            {

                while (true)
                {
                    var dPos = ApplyStep(step, team, bottom, pos);
                    if (!board.BoundaryCheck(dPos)) break;
                    if (board.IsTherePiece(dPos, out var piece))
                    {
                        if (piece.Team != team)
                            ways.Add(dPos);
                        break;
                    }
                    ways.Add(dPos);
                }
            }
            return ways;
        }


    }
}