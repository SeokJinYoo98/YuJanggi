using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Movement;

namespace Yujanggi.Utills.Board
{
    public static class BoardHelper
    {
        public static StepResult CheckCell(
            IBoardState board,
            PlayerType team,
            int dx,
            int dz)
        {
            if (!board.BoundaryCheck(dx, dz))
                return StepResult.Block;

            if (!board.IsTherePiece(dx, dz, out var pieceTeam, out _))
                return StepResult.Empty;

            if (pieceTeam == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }

        public static bool IsBottomPlayer(IBoardState board, PlayerType team)
        {
            return board.BottomPalyer == team;
        }
    }
}
