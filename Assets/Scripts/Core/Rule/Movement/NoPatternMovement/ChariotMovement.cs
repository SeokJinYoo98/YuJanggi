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
            
            foreach (var step in _steps)
            {
                var dPos = boardInfo.Pos;
                while (true)
                {
                    dPos = ApplyStep(step, team, bottom, dPos);
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