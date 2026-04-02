using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match.Movement;

namespace Yujanggi.Utills.Board
{
    public static class BoardHelper
    {
        public static StepResult CheckCell(
            IBoardModel board,
            PlayerTeam team,
            Pos pos)
        {
            if (!board.IsInside(pos))
                return StepResult.Block;

            if (!board.HasPiece(pos))
                return StepResult.Empty;

            if (board.GetPiece(pos).Team == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }
    }
}
