using System.Collections.Generic;
using Unity.VisualScripting;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Match.Movement
{
    public class CannonMovement : Movement
    {
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };

        public override List<Pos> FindWays(
            IBoardModel board,
            SelectionState selectPiece)
        {
            List<Pos> ways = new();
            var team    = selectPiece.SelectedPiece.Team;
            var start   = selectPiece.SelectedPos;

            foreach (var step in _steps)
            {
                bool bridge = false;
                var dPos = start;

                while (true)
                {
                    dPos = ApplyStep(step, team, dPos);

                    var result = CheckCell(board, team, dPos);

                    if (result == StepResult.Block)
                        break;

                    if (!bridge)
                    {
                        if (result == StepResult.Empty)
                            continue;

                        var piece = board.GetPiece(dPos);

                        if (piece.Type == PieceType.Cannon)
                            break;

                        bridge = true;
                        continue;
                    }

                    if (result == StepResult.Empty)
                    {
                        ways.Add(dPos);
                        continue;
                    }

                    if (result == StepResult.Enemy)
                    {
                        var piece = board.GetPiece(dPos);

                        if (piece.Type != PieceType.Cannon)
                            ways.Add(dPos);
                    }

                    break;
                }
            }

            return ways;
        }
    }
}