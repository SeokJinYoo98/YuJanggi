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
            SelectionState selectInfo)
        {
            List<Pos> ways = new();
            var bottom = selectInfo.BottomPlayer;
            var selectedPiece = selectInfo.SelectedPiece;
            var team = selectedPiece.Team;

            foreach (var step in _steps)
            {
                var dPos = selectInfo.SelectedPos;
                while (true)
                {
                    dPos = ApplyStep(step, team, bottom, dPos);
                    var result = CheckCell(board, team, dPos);
          
                    
                    if (result == StepResult.Block || result == StepResult.Team)
                        break;

                    ways.Add(dPos);
                    if (result == StepResult.Enemy)
                        break;
                }
            }
            return ways;
        }
    }
}